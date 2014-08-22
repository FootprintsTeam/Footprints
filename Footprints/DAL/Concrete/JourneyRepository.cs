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
            return  query.Count() == 0 ? null : query.First<Journey>();
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
                                                " CREATE (activity:Activity {ActivityID : {Activity}.ActivityID, Status : {Activity}.Status, Type : {Activity}.Type, JourneyID : {Activity}.JourneyID, Timestamp : {Activity}.Timestamp, UserName : user.UserName, FirstName : user.FirstName, LastName : user.LastName, ProfilePicURL : user.ProfilePicURL, Journey_Name : {Activity}.Journey_Name, Journey_Description : {Activity}.Journey_Description, Journey_NumberOfLike : {Activity}.Journey_NumberOfLike, Journey_NumberOfShare : {Activity}.Journey_NumberOfShare}) " +
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
                        Set("journey.Name = {Journey}.Name, journey.Description = {Journey}.Description, journey.TakenDate = {Journey}.TakenDate, journey.Timestamp = {Journey}.Timestamp, journey.NumberOfLike = {Journey}.NumberOfLike, journey.NumberOfShare = {Journey}.NumberOfShare").
                        WithParam("Journey", Journey).Return<Journey>("journey").Results;
            return query.Count<Journey>() > 0 ? true : false;
        }
        public bool DeleteJourney(Guid UserID, Guid JourneyID)
        {
            Db.Cypher.OptionalMatch("(User:User)").Where((User User) => User.UserID == UserID).
                      OptionalMatch("(User)-[rel:HAS]->(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                      OptionalMatch("(Journey:Journey)-[r]-()").
                      OptionalMatch("(Activity:Activity)").Where((Activity Activity) => Activity.JourneyID == JourneyID).
                      With("User, rel, r, Journey").
                      Where("rel IS NOT NULL").Set("Activity.Status = 'Deleted'").Delete("rel, r, Journey").ExecuteWithoutResults();
            return true;
        }
        public IList<Journey> GetJourneyList()
        {
            return Db.Cypher.Match("(journey:Journey)").Return(journey => journey.As<Journey>()).Results.ToList<Journey>();
        }
        public IList<Journey> GetJourneyListBelongToUser(Guid UserID)
        {
            return Db.Cypher.Match("(User:User)-[:HAS]->(journey:Journey)").Where((User User) => User.UserID == UserID).Return(journey => journey.As<Journey>()).Results.ToList<Journey>();
        }
        public void LikeJourney(Guid UserID, Guid JourneyID)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "LIKE_A_JOURNEY",
                JourneyID = JourneyID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:LIKED_BY]->(User) " +
                                                " SET Journey.NumberOfLike = Journey.NumberOfLike + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, Status : {Activity}.Status, Type : {Activity}.Type, JourneyID : {Activity}.JourneyID, Timestamp : {Activity}.Timestamp, UserName : User.UserName, FirstName : User.FirstName, LastName : User.LastName, ProfilePicURL : User.ProfilePicURL, Journey_Name : Journey.Name, Journey_Description : Journey.Description, Journey_NumberOfLike : Journey.NumberOfLike, Journey_NumberOfShare : Journey.NumberOfShare}) " +
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
            return Db.Cypher.Match("(Journey:Journey)-[:LIKED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(user => user.As<User>()).Results.ToList<User>();
        }
        public void ShareJourney(Guid UserID, Guid JourneyID, String Content)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "SHARE_A_JOURNEY",
                JourneyID = JourneyID,
                Content = Content,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:SHARED_BY]->(User) " +
                                                " SET Journey.NumberOfShare = Journey.NumberOfShare + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, Content : {Activity}.Content, Status : {Activity}.Status, Type : {Activity}.Type, UserName : User.UserName, FirstName : User.FirstName, LastName : User.LastName, ProfilePicURL : User.ProfilePicURL  JourneyID : {Activity}.JourneyID, Timestamp : {Activity}.Timestamp, Journey_Name : Journey.Name, Journey_Description : Journey.Description, Journey_NumberOfLike : Journey.NumberOfLike, Journey_NumberOfShare : Journey.NumberOfShare}) " +
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
            return Db.Cypher.Match("(Journey:Journey)-[:SHARED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(user => user.As<User>()).Results.ToList<User>();
        }
        //For Admin
        public IList<Journey> GetAllJourney()
        {
            return Db.Cypher.Match("(Journey:Journey)").Return(Journey => Journey.As<Journey>()).Results.ToList<Journey>();
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
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)-[rel:SHARED_BY]->(User:User)").Where((User User) => User.UserID == UserID)
                .AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).Return(Journey => Journey.As<Journey>())
                .Results.ToList<Journey>();
            return query.Count > 0 ? true : false;
        }
        public bool UserAlreadyShared(Guid UserID, Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)-[rel:SHARED_BY]->(User:User)").Where((User User) => User.UserID == UserID)
                .AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).Return(Journey => Journey.As<Journey>())
                .Results.ToList<Journey>();
            return query.Count > 0 ? true : false;
        }
        public bool UpdateJourney(Guid UserID, Guid JourneyID, String Name, String Description, DateTimeOffset TakenDate, DateTimeOffset Timestamp)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).AndWhere((User User) => User.UserID == UserID).
                        Set("Journey.Name = {Name}, Journey.Description = {Description}, Journey.TakenDate = {TakenDate}, Journey.Timestamp = {Timestamp}").
                        WithParams(new Dictionary<String, Object> { {"Name", Name}, {"Description", Description}, {"TakenDate", TakenDate}, {"Timestamp", Timestamp} }).
                        Return<Journey>("Journey").Results;
            return query.Count<Journey>() > 0 ? true : false;
        }
        public int GetNumberOfContent(Guid JourneyID)
        {
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content)").
                   Where((Journey Journey) => Journey.JourneyID == JourneyID).Return<int>("Count(Content)").Results;
            return query.Count() == 0 ? 0 : query.First();
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
                tmp = item.Journey;
                tmp.Destinations = new List<Destination>();
                foreach (var destination in item.Destination)
                {
                    tmp.Destinations.Add(destination.Data);   
                }
                result.Add(tmp);
            }
            return query.Count() == 0 ? null : result;
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
    }
}
