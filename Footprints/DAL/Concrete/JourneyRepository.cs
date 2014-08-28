using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
namespace Footprints.DAL.Concrete
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(IGraphClient client) : base(client) { }
        public Journey GetJourneyByID(Guid JourneyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").
                Where((Journey journey) => journey.JourneyID == JourneyID).
                Return(journey => journey.As<Journey>()).Results;
            return query.Count() == 0 ? null : query.First<Journey>();
        }
        public Journey GetJourneyDetail(Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                                  OptionalMatch("(Journey)-[:HAS]->(Destination:Destination)").
                                  OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                                  OptionalMatch("(Destination)-[:HAS]->(Content:Content)").
                                  With("Journey, Destination, Place, Content").OrderBy("Destination.OrderNumber").
                                  With("Journey, Destination, Place, Content").OrderBy("Content.Timestamp").
                                  Return((Journey, Destination, Place, Content) => new
                                  {
                                      journey = Journey.As<Journey>(),
                                      destination = Destination.As<Destination>(),
                                      place = Place.As<Place>(),
                                      content = Content.CollectAs<Content>()
                                  }
                                  ).Results;
            Journey result = null;
            Destination destination = new Destination();
            bool first = true;
            foreach (var item in query)
            {
                if (item.journey == null) return null;
                if (first)
                {
                    result = new Journey();
                    result = item.journey;
                    result.Destinations = new List<Destination>();
                    first = false;
                }
                if (item.destination != null)
                {
                    destination = item.destination;
                    destination.Place = new Place();
                    destination.Place = item.place;
                    destination.Contents = new List<Content>();
                    foreach (var content in item.content)
                    {
                        destination.Contents.Add(content.Data);
                    }
                    result.Destinations.Add(destination);
                }
            }
            return result;
        }
        public bool AddNewJourney(Guid UserID, Journey Journey)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Activity.StatusEnum.Active,
                Type = "ADD_NEW_JOURNEY",
                UserID = UserID,
                JourneyID = Journey.JourneyID,
                Journey_Name = Journey.Name,
                Journey_Description = Journey.Description,
                Journey_NumberOfLike = Journey.NumberOfLike,
                Journey_NumberOfShare = Journey.NumberOfShare,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery("CREATE (journey:Journey {journey}) " +
                                                " WITH journey " +
                                                " MATCH (user:User {UserID : {UserID}}) " +
                                                " CREATE (user)-[:HAS]->(journey) " +
                                                " CREATE (activity:Activity {Activity}) " +
                                                " SET activity.UserName = user.UserName, activity.FirstName = user.FirstName, activity.LastName = user.LastName, activity.ProfilePicURL = user.ProfilePicURL" + 
                                                " WITH user, journey, activity " +
                                                " MATCH (user)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (user)-[:LATEST_ACTIVITY]->(activity) " +
                                                " CREATE (activity)-[:NEXT]->(nextActivity) " +
                                                " CREATE (activity)-[:ACT_ON_JOURNEY]->(journey) " +
                                                " WITH user " +
                                                " MATCH (user)-[:FRIEND]->(friend) " +
                                                " WITH user, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(user)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, user, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  user " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(user) " +
                                                " CREATE (user)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser) ", new Dictionary<String, Object> { { "journey", Journey }, { "Activity", Activity }, { "UserID", UserID } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }
        public bool UpdateJourney(Guid UserID, Journey Journey)
        {
            var query = Db.Cypher.Match("(user:User)-[:HAS]->(journey:Journey)").Where((Journey journey) => journey.JourneyID == Journey.JourneyID).AndWhere((User user) => user.UserID == UserID).
                         Set("journey.Name = {Journey}.Name, journey.Description = {Journey}.Description, journey.TakenDate = {Journey}.TakenDate, journey.Timestamp = {Journey}.Timestamp, journey.NumberOfLike = {Journey}.NumberOfLike, journey.NumberOfShare = {Journey}.NumberOfShare").
                         WithParam("Journey", Journey).Return<Journey>("journey").Results;
            return query.Count<Journey>() > 0 ? true : false;
        }

        //ForAdmin
        public bool UpdateJourneyForAdmin(Journey Journey)
        {
            var query = Db.Cypher.Match("(journey:Journey)").Where((Journey journey) => journey.JourneyID == Journey.JourneyID).
                        Set("journey.Name = {Journey}.Name, journey.Description = {Journey}.Description").
                        WithParam("Journey", Journey).Return<Journey>("journey").Results;
            return query.Count<Journey>() > 0 ? true : false;
        }
        public bool DeleteJourney(Guid UserID, Guid JourneyID)
        {
            Db.Cypher.OptionalMatch("(User:User)").Where((User User) => User.UserID == UserID).
                      OptionalMatch("(User)-[rel:HAS]->(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                      OptionalMatch("(Journey:Journey)-[r]-()").
                      OptionalMatch("(Activity:Activity)").Where((Activity Activity) => Activity.JourneyID == JourneyID).
                      With("User, rel, r, Journey, Activity").
                      Where("rel IS NOT NULL").Set("Activity.Status = 'Deleted'").Delete("rel, r, Journey").ExecuteWithoutResults();
            return true;
        }
        public IList<Journey> GetJourneyList()
        {
            var query = Db.Cypher.Match("(journey:Journey)").Return(journey => journey.As<Journey>()).Results;
            return query.Count() == 0 ? null : query.ToList<Journey>();
        }
        public IList<Journey> GetJourneyListBelongToUser(Guid UserID)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(journey:Journey)").Where((User User) => User.UserID == UserID).Return(journey => journey.As<Journey>()).Results;
            return query.Count() == 0 ? null : query.ToList<Journey>();
        }
        public void LikeJourney(Guid UserID, Guid JourneyID)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "LIKE_A_JOURNEY",
                JourneyID = JourneyID,
                UserID = UserID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:LIKED_BY]->(User) " +
                                                " SET Journey.NumberOfLike = Journey.NumberOfLike + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, UserID : {Activity}.UserID, Status : {Activity}.Status, Type : {Activity}.Type, JourneyID : {Activity}.JourneyID, Timestamp : {Activity}.Timestamp, UserName : User.UserName, FirstName : User.FirstName, LastName : User.LastName, ProfilePicURL : User.ProfilePicURL, Journey_Name : Journey.Name, Journey_Description : Journey.Description, Journey_NumberOfLike : Journey.NumberOfLike, Journey_NumberOfShare : Journey.NumberOfShare}) " +
                                                " WITH User, Journey, Activity " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " CREATE (Activity)-[:ACT_ON_JOURNEY]->(Journey) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)",
                                                new Dictionary<String, Object> { { "UserID", UserID }, { "JoureyID", JourneyID }, { "Activity", Activity } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            // return true;
        }

        public void UnlikeJourney(Guid UserID, Guid JourneyID)
        {
            Db.Cypher.Match("(Journey:Journey)-[rel:LIKED_BY]->(User:User)").Where((User User) => User.UserID == UserID).AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).
                 Set("Journey.NumberOfLike = Journey.NumberOfLike - 1").Delete("rel")
                .ExecuteWithoutResults();
        }
        public IList<User> GetAllUserLiked(Guid JourneyID)
        {
            var query = Db.Cypher.Match("(Journey:Journey)-[:LIKED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(User => User.As<User>()).Results;
            return query.Count() == 0 ? null : query.ToList<User>();
        }
        public void ShareJourney(Guid UserID, Guid JourneyID, String Content)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "SHARE_A_JOURNEY",
                JourneyID = JourneyID,
                UserID = UserID,
                SharedContent = Content,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:SHARED_BY]->(User) " +
                                                " SET Journey.NumberOfShare = Journey.NumberOfShare + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, UserID : {Activity}.UserID, Content : {Activity}.Content, Status : {Activity}.Status, Type : {Activity}.Type, UserName : User.UserName, FirstName : User.FirstName, LastName : User.LastName, ProfilePicURL : User.ProfilePicURL  JourneyID : {Activity}.JourneyID, Timestamp : {Activity}.Timestamp, Journey_Name : Journey.Name, Journey_Description : Journey.Description, Journey_NumberOfLike : Journey.NumberOfLike, Journey_NumberOfShare : Journey.NumberOfShare}) " +
                                                " WITH User, Journey, Activity " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " CREATE (Activity)-[:ACT_ON_JOURNEY]->(Journey) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)",
                                                new Dictionary<String, Object> { { "UserID", UserID }, { "JoureyID", JourneyID }, { "Activity", Activity } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
        }
        public IList<User> GetAllUserShared(Guid JourneyID)
        {
            var query = Db.Cypher.Match("(Journey:Journey)-[:SHARED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(user => user.As<User>()).Results;
            return query.Count() == 0 ? null : query.ToList<User>();
        }
        //For Admin
        public IList<Journey> GetAllJourney()
        {
            var query = Db.Cypher.Match("(Journey:Journey)").Return(Journey => Journey.As<Journey>()).Results;
            return query.Count() == 0 ? null : query.ToList<Journey>();
        }
        public int GetNumberOfJourney()
        {
            return Db.Cypher.Match("(Journey:Journey)").Return<int>("Count(Journey)").Results.FirstOrDefault();
        }
        public int GetNumberOfLike(Guid JourneyID)
        {
            return Db.Cypher.Match("(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                Return<int>("Journey.NumberOfLike").Results.FirstOrDefault();
        }
        public int GetNumberOfShare(Guid JourneyID)
        {
            return Db.Cypher.Match("(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
               Return<int>("Journey.NumberOfShare").Results.FirstOrDefault();
        }
        public bool UserAlreadyLiked(Guid UserID, Guid JourneyID)
        {
            var query = Db.Cypher.Match("(Journey:Journey)-[rel:SHARED_BY]->(User:User)").Where((User User) => User.UserID == UserID)
                .AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).Return(Journey => Journey.As<Journey>())
                .Results;
            return query.Count() > 0 ? true : false;
        }
        public bool UserAlreadyShared(Guid UserID, Guid JourneyID)
        {
            var query = Db.Cypher.Match("(Journey:Journey)-[rel:SHARED_BY]->(User:User)").Where((User User) => User.UserID == UserID)
                .AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).Return(Journey => Journey.As<Journey>())
                .Results;
            return query.Count() > 0 ? true : false;
        }
        public bool UpdateJourney(Guid UserID, Guid JourneyID, String Name, String Description, DateTimeOffset TakenDate, DateTimeOffset Timestamp)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).AndWhere((User User) => User.UserID == UserID).
                        Set("Journey.Name = {Name}, Journey.Description = {Description}, Journey.TakenDate = {TakenDate}, Journey.Timestamp = {Timestamp}").
                        WithParams(new Dictionary<String, Object> { { "Name", Name }, { "Description", Description }, { "TakenDate", TakenDate }, { "Timestamp", Timestamp } }).
                        Return<Journey>("Journey").Results;
            return query.Count<Journey>() > 0 ? true : false;
        }
        public int GetNumberOfContent(Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content)").
                   Where((Journey Journey) => Journey.JourneyID == JourneyID).Return<int>("Count(Content)").Results;
            return query.Count() == 0 ? 0 : query.First();
        }

        public List<Content> GetAllContent(Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content)").
                   Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(Content => Content.As<Content>()).Results;
            return query.Count() == 0 ? null : query.ToList<Content>();
        }
        public IList<Journey> GetJourneyDetailsListBelongToUser(Guid UserID)
        {
            var query = Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)").Where((User User) => User.UserID == UserID).
                        OptionalMatch("(Journey)-[:HAS]->(Destination:Destination)").
                        With("Journey, Destination").OrderBy("Destination.OrderNumber").
                        Return((Journey, Destination) => new
                        {
                            Journey = Journey.As<Journey>(),
                            Destination = Destination.CollectAs<Destination>()
                        }).Results;
            List<Journey> result = new List<Journey>();
            Journey tmp = new Journey();
            foreach (var item in query)
            {
                if (item.Journey == null) return null;
                else tmp = item.Journey;
                tmp.Destinations = new List<Destination>();
                foreach (var destination in item.Destination)
                {
                    tmp.Destinations.Add(destination.Data);
                }
                result.Add(tmp);
            }
            return query.Count() == 0 ? null : result;
        }
        public Journey GetJourneyDetailWithComment(Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)").
                Where((Journey Journey) => Journey.JourneyID == JourneyID).
                OptionalMatch("(Journey)-[:HAS]->(Destination:Destination)").
                OptionalMatch("(DComment:Comment)-[:ON]->(Destination)").
                OptionalMatch("(JComment:Comment)-[:ON]->(Journey)").
                OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                OptionalMatch("(DComment)-[:COMMENT_BY]->(DUser:User)").
                OptionalMatch("(JComment)-[:COMMENT_BY]->(JUser:User)").
                With("Journey, Destination, DComment, JComment, Place, DUser, JUser").
                OrderBy("Destination.OrderNumber, DComment.Timestamp, JComment.Timestamp").
                Return((Journey, Destination, DComment, JComment, Place, DUser, JUser) => new
                {
                    Journey = Journey.As<Journey>(),
                    Destination = Destination.As<Destination>(),
                    JComment = JComment.As<Comment>(),
                    DComment = DComment.As<Comment>(),
                    Place = Place.As<Place>(),
                    DUser = DUser.As<User>(),
                    JUser = JUser.As<User>()
                }).Results;
            Destination destination = new Destination();
            Journey result = null;
            bool first = true;
            foreach (var item in query)
            {
                if (item.Journey == null)
                {
                    return null;
                }
                if (first)
                {
                    result = item.Journey;
                    result.Destinations = new List<Destination>();
                    result.Comments = new List<Comment>();
                    first = false;
                }
                if (item.JComment != null)
                {
                    if (item.JUser != null) item.JComment.User = item.JUser; 
                    result.Comments.Add(item.JComment);
                }
                
                if (item.Destination != null)
                {
                    if (!item.Destination.DestinationID.Equals(destination.DestinationID))
                    {
                        destination = item.Destination;
                        result.Destinations.Add(destination);
                        destination.Place = item.Place;
                        destination.Comments = new List<Comment>();
                    }
                    if (item.DComment != null)
                    {
                        if (item.DUser != null) item.DComment.User = item.DUser;
                        destination.Comments.Add(item.DComment);
                    }
                }               
            }
            return query.Count() == 0 ? null : result;
        }

        public long GetNumberOfCreatedJourneyBetweenDays(String Start, String End)
        {
            Start = Start.Replace("{", "");
            Start = Start.Replace("}", "");
            End = End.Replace("{", "");
            End = End.Replace("}", "");
           var query = Db.Cypher.Match("(Journey:Journey)").
           Where("Journey.Timestamp >= {Start}").WithParam("Start", Start).
           AndWhere("Journey.Timestamp <= {End}").WithParam("End", End).
           Return((Journey) => new
           {
               NumberOfJourney = Journey.Count()
           }).Results;
            foreach (var item in query)
            {
                return item.NumberOfJourney;
            }
            return 0;
        }
        public bool DeleteJourneyForAdmin(Guid JourneyID) 
        {
            Db.Cypher.Match("(Journey:Journey)-[r]-()").
                        Where((Journey Journey) => Journey.JourneyID == JourneyID).
                        Match("(Activity:Activity)").Where((Activity Activity) => Activity.JourneyID == JourneyID).
                        Set("Activity.Status = 'Deleted'").
                        With("Journey, r").
                        Delete("Journey, r").ExecuteWithoutResults();
            return true;
        }
    }
    public interface IJourneyRepository : IRepository<Journey>
    {
        bool AddNewJourney(Guid UserID, Journey Journey);
        Journey GetJourneyByID(Guid JourneyID);
        Journey GetJourneyDetail(Guid JourneyID);
        bool UpdateJourney(Guid UserID, Journey Journey);
        bool DeleteJourney(Guid UserID, Guid JourneyID);
        IList<Journey> GetJourneyList();
        IList<Journey> GetJourneyListBelongToUser(Guid UserID);
        void LikeJourney(Guid UserID, Guid JourneyID);
        void UnlikeJourney(Guid UserID, Guid JourneyID);
        IList<User> GetAllUserLiked(Guid JourneyID);
        void ShareJourney(Guid UserID, Guid JourneyID, String Content);
        IList<User> GetAllUserShared(Guid JourneyID);
        IList<Journey> GetAllJourney();
        int GetNumberOfJourney();
        int GetNumberOfLike(Guid JourneyID);
        int GetNumberOfShare(Guid JourneyID);
        bool UserAlreadyLiked(Guid UserID, Guid JourneyID);
        bool UserAlreadyShared(Guid UserID, Guid JourneyID);
        bool UpdateJourneyForAdmin(Journey Journey);
        bool UpdateJourney(Guid UserID, Guid JourneyID, String Name, String Description, DateTimeOffset TakenDate, DateTimeOffset Timestamp);
        int GetNumberOfContent(Guid JourneyID);
        IList<Journey> GetJourneyDetailsListBelongToUser(Guid UserID);
        Journey GetJourneyDetailWithComment(Guid JourneyID);
        long GetNumberOfCreatedJourneyBetweenDays(String Start, String End);
        bool DeleteJourneyForAdmin(Guid JourneyID);
        List<Content> GetAllContent(Guid JourneyID);
    }
}
