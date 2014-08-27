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
                result.Contents = new List<Content>();
                result.Place = item.Place;
                foreach (var content in item.Contents)
                {
                    result.Contents.Add(content.Data);
                }
                return result;
            }
            return null;
        }
        public Destination GetDestinationDetailWithLimitedContent(Guid DestinationID, int Limit)
        {
            var query = Db.Cypher.OptionalMatch("(Destination:Destination)").
                        Where((Destination Destination) => Destination.DestinationID == DestinationID).
                        OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                        OptionalMatch("(Destination)-[:HAS]->(Content:Content)").
                        With("Destination, Place, Content").OrderBy("Content.Timestamp").
                        With("Destination, Place, Content").Limit(Limit).
                        Return((Destination, Place, Content) => new
                        {
                            Destination = Destination.As<Destination>(),
                            Place = Place.As<Place>(),
                            Contents = Content.CollectAs<Content>()
                        }).Results;
            Destination result = new Destination();            
            foreach (var item in query)
            {
                result = item.Destination;
                result.Place = new Place();
                result.Contents = new List<Content>();
                if (item.Place != null) result.Place = item.Place;
                foreach (var content in item.Contents)
                {
                    result.Contents.Add(content.Data);
                }
                return result;
            }
            return null;
        }
        public bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID)
        {
            Activity Activity = new Activity()
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "ADD_NEW_DESTINATION",
                JourneyID = JourneyID,
                DestinationID = Destination.DestinationID,
                Destination_Name = Destination.Name,
                Destination_Description = Destination.Description,
                Destination_NumberOfLike = Destination.NumberOfLike,
                Destination_NumberOfShare = Destination.NumberOfShare,
                UserID = UserID,
                PlaceID = Place.PlaceID,
                Place_Name = Place.Name,
                Place_Address = Place.Address,
                Longitude = Place.Longitude,
                Latitude = Place.Latitude,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" OPTIONAL MATCH (User:User)-[:HAS]->(Journey:Journey) WHERE (User.UserID = {UserID}) AND (Journey.JourneyID = {JourneyID})" +
                                                " WITH User, Journey " +
                                                " WHERE (Journey IS NOT NULL) AND (User IS NOT NULL) " +
                                                " CREATE (Destination:Destination {Destination}) " +
                                                " MERGE (Place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference, Address : {Place}.Address } ) " +
                                                " CREATE (Destination)-[:AT]->(Place) " +
                                                " CREATE (Journey)-[:HAS]->(Destination) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURL, Activity.Journey_Name = Journey.Name, Activity.Journey_Description = Journey.Description, Activity.Journey_NumberOfLike = Journey.NumberOfLike, Activity.Journey_NumberOfShare = Journey.NumberOfShare " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination)" +
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
                Merge("(place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference, Address : {Place}.Address } )").
                WithParam("Place", Destination.Place).
                With("destination, place").
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.DestinationID == Destination.DestinationID).
                Set("Activity.Place_Name = place.Name, Activity.Place_Address = place.Address, Activity.Longitude = place.Longitude, Activity.Latitude = place.Latitude ").
                With("destination, place").
                Match("(destination)-[rel:AT]->(Place:Place)").
                Delete("rel").
                Create("(destination)-[:AT]->(place)").
                Return(destination => destination.As<Destination>()).Results;
            return query.Count<Destination>() > 0 ? true : false;
        }

        public bool UpdateDestination(Guid UserID, Guid DestinationID, String Name, String Description, DateTimeOffset TakenDate, Place Place, DateTimeOffset Timestamp)
        {
            var query = Db.Cypher.OptionalMatch("(user:User)-[:HAS]->(Journey:Journey)-[:HAS]->(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).AndWhere((User user) => user.UserID == UserID).
                Set("destination.Name = {Name}, destination.Description = {Description}, destination.TakenDate = {TakenDate}, destination.Timestamp = {Timestamp}").
                WithParams(new Dictionary<String, Object> {{"Name", Name}, {"Description", Description}, {"TakenDate", TakenDate}, {"Timestamp", Timestamp}}).
                Merge("(place:Place {PlaceID : {Place}.PlaceID, Name : {Place}.Name, Longitude : {Place}.Longitude, Latitude : {Place}.Latitude, Reference : {Place}.Reference, Address : {Place}.Address } )").
                WithParam("Place", Place).
                With("destination, place").
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.DestinationID == DestinationID).
                Set("Activity.Place_Name = place.Name, Activity.Place_Address = place.Address, Activity.Longitude = place.Longitude, Activity.Latitude = place.Latitude ").                
                With("destination, place").
                Match("(destination)-[rel:AT]->(Place:Place)").
                Delete("rel").
                Create("(destination)-[:AT]->(place)").
                Return(destination => destination.As<Destination>()).Results;
            return query.Count<Destination>() > 0 ? true : false;
        }

        public bool UpdateDestinationForAdmin(Destination Destination)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == Destination.DestinationID).
                        Set("destination.Name = {Destination}.Name, destination.Description = {Destination}.Description").
                        WithParam("Destination", Destination).
                        Return(destination => destination.As<Destination>()).Results;
            return query.Count<Destination>() > 0 ? true : false;
        }
        public void DeleteDestination(Guid UserID, Guid DestinationID)
        {
            Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)").
                Where((Destination Destination) => Destination.DestinationID == DestinationID).
                AndWhere((User User) => User.UserID == UserID).
                OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                OptionalMatch("(Destination)-[:HAS]->(Content:Content)").
                OptionalMatch("(Destination)-[r]-()").
                OptionalMatch("(Activity:Activity)").
                Where((Activity Activity) => Activity.DestinationID == DestinationID).
                Set("Activity.Status = 'Deleted'").
                Delete("Destination, r, Place, Content").
                ExecuteWithoutResults();
        }
        //For Admin
        public void DeleteDestinationForAdmin(Guid DestinationID)
        {
            Db.Cypher.OptionalMatch("(Destination:Destination)-[r]-()").
                Where((Destination Destination) => Destination.DestinationID == DestinationID).
                OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                OptionalMatch("(Destination)-[:HAS]->(Content:Content)").
                OptionalMatch("(Activity:Activity)").
                Where((Activity Activity) => Activity.DestinationID == DestinationID).
                Set("Activity.Status = 'Deleted'").
                Delete("Destination, r, Place, Content").
                ExecuteWithoutResults();
        }
        public void AddNewContent(Content Content, Guid DestinationID, Guid UserID)
        {
            Activity Activity = new Activity()
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "ADD_NEW_CONTENT",
                DestinationID = DestinationID,
                ContentID = Content.ContentID,
                ContentURL = Content.URL,
                UserID = UserID,
                Timestamp = DateTimeOffset.Now              
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination) WHERE (User.UserID = {UserID}) AND (Destination.DestinationID = {DestinationID}) " +
                                                " MATCH (Destination)-[:AT]->(Place:Place)" +
                                                " WITH User, Journey, Destination, Place " +
                                                " WHERE (Destination IS NOT NULL) AND (User IS NOT NULL) " +
                                                " CREATE (Content:Content {Content}) " +                                               
                                                " CREATE (Destination)-[:HAS]->(Content) " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, ContentID : {Activity}.ContentID, ContentURL : {Activity}.ContentURL, UserID : {Activity}.UserID, Status : {Activity}.Status, Type : {Activity}.Type, DestinationID : {Activity}.DestinationID, Timestamp : {Activity}.Timestamp, Destination_Name : Destination.Name, Destination_Description : Destination.Description, Destination_NumberOfLike : Destination.NumberOfLike, Destination_NumberOfShare : Destination.NumberOfShare}) " +
                                                " SET Activity.Journey_Name = Journey.Name, Activity.Journey_Description = Journey.Description, Activity.Journey_NumberOfLike = Journey.NumberOfLike, Activity.Journey_NumberOfShare = Journey.NumberOfShare " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURL" +
                                                " SET Activity.Place_Name = Place.Name, Activity.Place_Address = Place.Address, Activity.Longitude = Place.Longitude, Activity.Latitude = Place.Latitude" +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination) " + 
                                                " WITH Destination, Content, Activity, User " +
                                                " MATCH (Destination)-[:AT]->(Place:Place)" + 
                                                " SET Activity.Place_Name = Place.Name, Activity.Place_Address = Place.Address, Activity.Longitude = Place.Longitude, Activity.Latitude = Place.Latitude " +
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
            Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:HAS]->(Content:Content)").
                Where((Content Content) => Content.ContentID == ContentID).
                AndWhere((User User) => User.UserID == UserID).
                Match("(Content)-[r]-()").
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.ContentID == ContentID).
                Set("Activity.Status = 'Deleted'").
                Delete("Content, r").
                ExecuteWithoutResults();
        }
        public IList<Content> GetAllContent(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(Content => Content.As<Content>()).Results;
            return query.Count() == 0 ? null : query.ToList<Content>();
        }
        public void LikeDestination(Guid UserID, Guid DestinationID)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "LIKE_A_DESTINATION",
                DestinationID = DestinationID,
                UserID = UserID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Destination:Destination) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Destination.DestinationID = {DestinationID} ) " +
                                                " MATCH (Destination)-[:AT]->(Place:Place)" + 
                                                " CREATE (Destination)-[:LIKED_BY]->(User) " +
                                                " SET Destination.NumberOfLike = Destination.NumberOfLike + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, UserID : {Activity}.UserID, Status : {Activity}.Status, Type : {Activity}.Type, DestinationID : {Activity}.DestinationID, Timestamp : {Activity}.Timestamp, Destination_Name : Destination.Name, Destination_Description : Destination.Description, Destination_NumberOfLike : Destination.NumberOfLike, Destination_NumberOfShare : Destination.NumberOfShare}) " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURL " +
                                                " SET Activity.Place_Name = Place.Name, Activity.Place_Address = Place.Address, Activity.Longitude = Place.Longitude, Activity.Latitude = Place.Latitude " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination) " +
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
            var query = Db.Cypher.Match("(Destination:Destination)-[:LIKED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(User => User.As<User>()).Results;
            return query.Count() == 0 ? null : query.ToList<User>();
        }
        public void ShareDestination(Guid UserID, Guid DestinationID, String Content)
        {

            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Models.Activity.StatusEnum.Active,
                Type = "SHARE_A_DESTINATION",
                DestinationID = DestinationID,
                UserID = UserID,
                SharedContent = Content,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Destination:Destination) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Destination.DestinationID = {DestinationID} ) " +
                                                " MATCH (Destination)-[:AT]->(Place:Place)" +
                                                " CREATE (Destination)-[:SHARED_BY]->(User) " +
                                                " SET Destination.NumberOfShare = Destination.NumberOfShare + 1 " +
                                                " CREATE (Activity:Activity {ActivityID : {Activity}.ActivityID, UserID : {Activity}.UserID, Content : {Activity}.Content, Status : {Activity}.Status, Type : {Activity}.Type, DestinationID : {Activity}.DestinationID, Timestamp : {Activity}.Timestamp, Destination_Name : Destination.Name, Destination_Description : Destination.Description, Destination_NumberOfLike : Destination.NumberOfLike, Destination_NumberOfShare : Destination.NumberOfShare}) " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURL " +
                                                " SET Activity.Place_Name = Place.Name, Activity.Place_Address = Place.Address, Activity.Longitude = Place.Longitude, Activity.Latitude = Place.Latitude " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination)" +
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
            var query = Db.Cypher.Match("(Destination:Destination)-[:SHARED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results;
            return query.Count() == 0 ? null : query.ToList<User>();
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
            return query.Count() == 0 ? null : result;
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
        public int GetNumberOfContentInDestination(Guid DestinationID)
        {
            var query = Db.Cypher.OptionalMatch("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return<int>("Count(Content)").Results;
            return query.Count() == 0 ? 0 : query.First();
        }
        public IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID)
        {
            var query = Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").
                        Where((Destination Destination) => Destination.DestinationID == DestinationID).
                        Return(Content => Content.As<Content>()).OrderBy("Content.Timestamp").Skip(Skip).Limit(Limit).Results;
            return query.Count() == 0 ? null : query.ToList<Content>();
        }
        public int GetMaxOrderNumber(Guid JourneyID)
        {
            var query = Db.Cypher.Match("(Journey:Journey)-[:HAS]->(Destination:Destination)").
                Where((Journey Journey) => Journey.JourneyID == JourneyID).
                With("Destination").OrderByDescending("Destination.OrderNumber").Limit(1).Return((Destination) => new
                {
                    MaxOrderNumber = Destination.As<Destination>().OrderNumber
                }).Results;
            foreach (var item in query)
            {
                return item.MaxOrderNumber;
            }
            return 0;
        }
        public Destination GetDestinationDetailWithComment(Guid DestinationID, int Limit)
        {
            var query = Db.Cypher.Match("(Destination:Destination)").
                        Where((Destination Destination) => Destination.DestinationID == DestinationID).
                        Match("(Destination)-[:AT]->(Place:Place)").
                        Match("(Comment:Comment)-[:ON]->(Destination)").
                        Return((Destination, Place, Comment) => new
                        {
                            Destination = Destination.As<Destination>(),
                            Place = Place.As<Place>(),
                            Comment = Comment.As<Comment>()
                        }).Limit(Limit).Results;
            Destination result = new Destination();
            bool first = true;
            foreach (var item in query)
            {
                if (item.Destination == null) return null;
                if (first)
                {
                    result = item.Destination;
                    result.Place = new Place();
                    result.Place = item.Place;
                    result.Comments = new List<Comment>();
                }
                result.Comments.Add(item.Comment);
            }
            return query.Count() == 0 ? null : result;
        }
        //For Admin
        public long GetNumberOfCreatedDestinationBetweenDays(String Start, String End)
        {
            var query = Db.Cypher.Match("(Destination:Destination)").
                       Where((Destination Destination) => Destination.Timestamp.ToString().CompareTo(Start) >= 0).
                       AndWhere((Destination Destination) => Destination.Timestamp.ToString().CompareTo(End) <= 0).
                       Return((Destination) => new
                       {
                           NumberOfDestination = Destination.Count()
                       }).Results;
            foreach (var item in query)
            {
                return item.NumberOfDestination;
            }
            return 0;
        }
        //For Admin
        public long GetNumberOfDestination()
        {
            var query = Db.Cypher.Match("(Destination:Destination)").Return((Destination) => new
            {
                NumberOfDestination = Destination.Count()
            }).Results;
            foreach (var item in query)
            {
                return item.NumberOfDestination;
            }
            return 0;
        }
    }
    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        bool UserAlreadyLike(Guid userID, Guid destinationID);
        Place GetDestinationPlace(Guid DestinationID);
        Destination GetDestination(Guid DestinationID);
        Destination GetDestinationDetail(Guid DestinationID);
        Destination GetDestinationDetailWithLimitedContent(Guid DestinationID, int Limit);
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
        int GetNumberOfContentInDestination(Guid DestinationID);
        IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID);
        void DeleteDestinationForAdmin(Guid DestinationID);
        bool UpdateDestination(Guid UserID, Guid DestinationID, String Name, String Description, DateTimeOffset TakenDate, Place Place, DateTimeOffset Timestamp);
        int GetMaxOrderNumber(Guid JourneyID);
        Destination GetDestinationDetailWithComment(Guid DestinationID, int Limit);
        long GetNumberOfCreatedDestinationBetweenDays(String Start, String End);
        long GetNumberOfDestination();
    }

}
