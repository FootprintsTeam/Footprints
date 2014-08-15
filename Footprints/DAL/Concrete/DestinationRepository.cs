using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
using Footprints.DAL.Concrete;
using Neo4jClient.Cypher;

namespace Footprints.DAL.Concrete
{
    public class DestinationRepository : RepositoryBase<DestinationRepository>, IDestinationRepository
    {
        public DestinationRepository(IGraphClient client) : base(client) { }
        public Destination GetDestination(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.Count() == 0 ? null : query.Results.First<Destination>();
        }
        public Place GetDestinationPlace(Guid DestinationID) {
            var result = Db.Cypher.Match("(destination:Destination)-[:AT]->(place:Place)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(place => place.As<Place>()).Results;

            return result.Count() == 0 ? null : result.First();   
        }
        public bool UserAlreadyLike(Guid userID, Guid destinationID) {
            var result = Db.Cypher.Match("(destination:Destination)").
                Match("(user:User)").Where("destination-[:LIKED_BY]->user").
                AndWhere((User user) => user.UserID == userID).
                AndWhere((Destination destination) => destination.DestinationID == destinationID).
                Return(destination => destination.As<Destination>()).Results.ToList<Destination>();
            return result.Count > 0 ? true : false;
        }
        public Destination GetDestinationDetail(Guid DestinationID)
        {
            var query = Db.Cypher.OptionalMatch("(destination:Destination)-[:AT]->(place:Place)").Where((Destination destination) => destination.DestinationID == DestinationID).
                OptionalMatch("(destination)-[:HAS]->(content:Content)").
                With("destination, place, content").OrderBy("content.Timestamp").
                Return((destination, place, content) => new
                {
                    Destination = destination.As<Destination>(),
                    Place = place.As<Place>(),
                    Contents = content.CollectAs<Content>()
                }).Results;
            Destination result = new Destination();
            foreach (var item in query)
            {
                result = item.Destination;
                result.Place = new Place();
                result.Place = item.Place;
                result.Contents = new List<Content>();
                foreach (var content in item.Contents)
                {
                    result.Contents.Add(content.Data);
                }
                return result;
            }
            return result;
        }
        public bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID)
        {
            Activity Activity = new Activity()
            {
                ActivityID = Guid.NewGuid(),
                Type = "ADD_NEW_DESTINATION",
                JourneyID = JourneyID,
                DestinationID = Destination.DestinationID,
                UserID = UserID,
                PlaceID = Place.PlaceID
            };
            CypherQuery query = new CypherQuery(" OPTIONAL MATCH (User:User)-[:HAS]->(Journey:Journey) WHERE (User.UserID = {UserID}) AND (Journey.JourneyID = {JourneyID})" +
                                                " WITH User, Journey " +
                                                " WHERE (Journey IS NOT NULL) AND (User IS NOT NULL) " +
                                                " CREATE (Destination:Destination {Destination}) " +
                                                " MERGE (Place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference } ) " +
                                                " CREATE (Destination)-[:AT]->(Place) " +
                                                " CREATE (Journey)-[:HAS]->(Destination) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Destination, Journey, Activity, User " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(user)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(user) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser) ",
                                                new Dictionary<String, Object> { { "Destination", Destination }, { "UserID", UserID }, { "JourneyID", JourneyID }, { "Activity", Activity }, { "Place", Place } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }
        public bool UpdateDestination(Guid UserID, Destination Destination)
        {
            var query = Db.Cypher.OptionalMatch("(user:User)-[:HAS]->(Journey:Journey)-[:HAS]->(destination:Destination)").Where((Destination destination) => destination.DestinationID == Destination.DestinationID).AndWhere((User user)=>user.UserID == UserID).
                Set("destination.Name = {Destination}.Name, destination.OrderNumber = {Destination}.OrderNumber, destination.Description = {Destination}.Description, destination.TakenDate = {Destination}.TakenDate, destination.NumberOfLike = {Destination}.NumberOfLike, destination.NumberOfShare = {Destination}.NumberOfShare, destination.Timestamp = {Destination}.Timestamp").
                WithParam("Destination", Destination).
                Merge("(place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference } )").WithParam("Place", Destination.Place).
                Merge("(destination)-[:AT]->(place)").
                Return(destination => destination.As<Destination>()).Results;
            return query.Count<Destination>() > 0 ? true : false;
        }
        public bool UpdateDestinationForAdmin(Destination Destination)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == Destination.DestinationID).
                Set("destination.Name = {Destination}.Name, destination.OrderNumber = {Destination}.OrderNumber, destination.Description = {Destination}.Description, destination.TakenDate = {Destination}.TakenDate, destination.NumberOfLike = {Destination}.NumberOfLike, destination.NumberOfShare = {Destination}.NumberOfShare, destination.Timestamp = {Destination}.Timestamp").
                WithParam("Destination", Destination).
                Merge("(place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference } )").WithParam("Place", Destination.Place).
                Merge("(destination)-[:AT]->(place)").
                Return(destination => destination.As<Destination>()).Results;
            return query.Count<Destination>() > 0 ? true : false;
        }
        public void DeleteDestination(Guid UserID, Guid DestinationID)
        {
            Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[r]->(n)").Where((Destination Destination) => Destination.DestinationID == DestinationID).AndWhere((User User) => User.UserID == UserID).
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.DestinationID == DestinationID).Set("Activity.Status = 'DELETED'").Delete("Destination, r, n").ExecuteWithoutResults();
        }
        //For Admin
        public void DeleteDestinationForAdmin(Guid DestinationID)
        {
            Db.Cypher.OptionalMatch("(Destination:Destination)-[r]->(n)").
                Where((Destination Destination) => Destination.DestinationID == DestinationID).
                Match("(Activity:Activity)").
                Where((Activity Activity) => Activity.DestinationID == DestinationID).
                Set("Activity.Status = 'DELETED'").
                Delete("Destination, r, n").
                ExecuteWithoutResults();
        }
        public void AddNewContent(Content Content, Guid DestinationID, Guid UserID)
        {
            Activity Activity = new Activity()
            {
                ActivityID = Guid.NewGuid(),
                Type = "ADD_NEW_CONTENT",
                DestinationID = DestinationID,
                ContentID = Content.ContentID
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination) WHERE (User.UserID = {UserID}) AND (Destination.DestinationID = {DestinationID}) " +
                                                " WITH User, Destination " +
                                                " WHERE (Destination IS NOT NULL) AND (User IS NOT NULL) " +
                                                " CREATE (Content:Content {Content}) " +                                               
                                                " CREATE (Destination)-[:HAS]->(Content) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Destination, Content, Activity, User " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(user)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(user) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser) ",
                                                new Dictionary<String, Object> { { "DestinationID", DestinationID }, { "UserID", UserID }, { "Content", Content }, { "Activity", Activity } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);            
        }
        public void UpdateContent(Guid UserID, Content Content)
        {
            Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content:Content)").Where((Content content) => content.ContentID == Content.ContentID).AndWhere((User user)=> user.UserID == UserID).
                        Set("Content = {Content}").WithParam("Content", Content).ExecuteWithoutResults();
        }
        public void DeleteContent(Guid UserID, Guid ContentID)
        {
            Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content:Content)-[r]-()").Where((Content Content) => Content.ContentID == ContentID).AndWhere((User User) => User.UserID == UserID).
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.ContentID == ContentID).Set("Activity.Status = 'DELETED'").Delete("Content, r").ExecuteWithoutResults();
        }
        public IList<Content> GetAllContent(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(content => content.As<Content>()).Results.ToList<Content>();
        }
        public void LikeDestination(Guid UserID, Guid DestinationID)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "LIKE_A_DESTINATION",
                DestinationID = DestinationID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Destination:Destination) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Destination.DestinationID = {DestinationID} ) " +
                                                " CREATE (Destination)-[:LIKED_BY]->(User) " +
                                                " SET Destination.NumberOfLike = Destination.NumberOfLike + 1 " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH User, Destination, Activity " +
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
                                                new Dictionary<String, Object> { { "UserID", UserID }, { "DestinationID", DestinationID }, { "Activity", Activity } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
        }
        public void UnlikeDestination(Guid UserID, Guid DestinationID)
        {

            Db.Cypher.Match("(Destination:Destination)-[rel:LIKED_BY]->(User:User)").Where((User User) => User.UserID == UserID).AndWhere((Destination Destination) => Destination.DestinationID == DestinationID).
                 Set("Destination.NumberOfLike = Destination.NumberOfLike - 1").Delete("rel")
                .ExecuteWithoutResults();
        }
        public IList<User> GetAllUserLiked(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:LIKED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results.ToList<User>();
        }
        public void ShareDestination(Guid UserID, Guid DestinationID, String Content)
        {

            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "SHARE_A_DESTINATION",
                DestinationID = DestinationID,
                Content = Content,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Destination:Destination) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Destination.DestinationID = {DestinationID} ) " +
                                                " CREATE (Destination)-[:SHARED_BY]->(User) " +
                                                " SET Destination.NumberOfShare = Destination.NumberOfShare + 1 " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH User, Destination, Activity " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination) " +
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
                                                new Dictionary<String, Object> { { "UserID", UserID }, { "DestinationID", DestinationID }, { "Activity", Activity } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
        }
        public IList<User> GetAllUserShared(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:SHARED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results.ToList<User>();
        }
        //For Admin
        public IList<Destination> GetAllDestination() 
        {
            var query = Db.Cypher.OptionalMatch("(destination:Destination)-[:AT]->(place:Place)").
                                  OptionalMatch("(destination)-[:HAS]->(content:Content)").
                                  With("destination, place, content").OrderBy("content.Timestamp").
                                    Return((destination, place, content) => new
                                    {
                                        destination = destination.As<Destination>(),
                                        place = place.As<Place>(),
                                        content = content.CollectAs<Content>()
                                    }).Results;
            List<Destination> result = new List<Destination>();       
            Destination currentDestination = new Destination();           
            foreach (var item in query)
            {
                currentDestination = new Destination();
                if (item.destination != null) currentDestination = item.destination;
                currentDestination.Contents = new List<Content>();
                currentDestination.Place = new Place();
                if (item.place != null) currentDestination.Place = item.place;
                foreach (var content in item.content)
                {
                    if (content != null) currentDestination.Contents.Add(content.Data);
                }
                result.Add(currentDestination);
            }
            return result;
        }
        public int GetNumberOfDestination(Guid UserID)
        {
            var query = Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)").Where((User User) => User.UserID == UserID).Return<int>("Count(Destination)").Results;
            return query.Count() == 0 ? 0 : query.First();
        }
        public int GetNumberOfLike(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(Destination:Destination)").Where((Destination Destination) => Destination.DestinationID == DestinationID).
                Return<int>("Destination.NumberOfLike").Results;
            return query.Count() == 0 ? 0 : query.First();
        }
        public int GetNumberOfShare(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(Destination:Destination)").Where((Destination Destination) => Destination.DestinationID == DestinationID).
                Return<int>("Destination.NumberOfShare").Results;
            return query.Count() == 0 ? 0 : query.First();
        }
        public bool UserAlreadyShared(Guid UserID, Guid DestinationID)
        {
            var query = Db.Cypher.OptionalMatch("(Destination:Destination)-[rel:SHARED_BY]->(User:User)").Where((User User) => User.UserID == UserID)
                .AndWhere((Destination Destination) => Destination.DestinationID == DestinationID).Return(Destination => Destination.As<Destination>())
                .Results.ToList<Destination>();
            return query.Count > 0 ? true : false;
        }
        public int GetNumberOfContent(Guid UserID)
        {
            var query = Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content:Content)").Where((User User) => User.UserID == UserID).Return<int>("Count(Content)").Results;
            return query.Count() == 0 ? 0 : query.First();
        }
        public IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID)
        {
            var query = Db.Cypher.OptionalMatch("(Destination:Destination)-[:HAS]->(Content:Content)").
                        Where((Destination Destination) => Destination.DestinationID == DestinationID).
                        Return(Content => Content.As<Content>()).OrderBy("Content.Timestamp").Skip(Skip).Limit(Limit).Results;
            return query.Count() == 0 ? null : query.ToList<Content>();
        }
    }
    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        bool UserAlreadyLike(Guid userID, Guid destinationID);
        Place GetDestinationPlace(Guid DestinationID);
        Destination GetDestination(Guid DestinationID);
        Destination GetDestinationDetail(Guid DestinationID);
        bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID);
        bool UpdateDestination(Guid UserID, Destination Destination);
        bool UpdateDestinationForAdmin(Destination Destination);
        void DeleteDestination(Guid UserID, Guid DestinationID);
        void AddNewContent(Content Content, Guid DestinationID, Guid UserID);
        void UpdateContent(Guid UserID, Content Content);
        void DeleteContent(Guid UserID, Guid ContentID);
        IList<Content> GetAllContent(Guid DestinationID);
        void LikeDestination(Guid UserID, Guid DestinationID);
        void UnlikeDestination(Guid UserID, Guid DestinationID);
        IList<User> GetAllUserLiked(Guid DestinationID);
        void ShareDestination(Guid UserID, Guid DestinationID, String Content);
        IList<User> GetAllUserShared(Guid DestinationID);
        IList<Destination> GetAllDestination();
        int GetNumberOfDestination(Guid UserID);
        int GetNumberOfLike(Guid DestinationID);
        int GetNumberOfShare(Guid DestinationID);
        bool UserAlreadyShared(Guid UserID, Guid DestinationID);
        int GetNumberOfContent(Guid UserID);
        IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID);
        void DeleteDestinationForAdmin(Guid DestinationID);
    }

}
