﻿using System;
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
            return query.Results.First<Destination>();
        }

        public Destination GetDestinationDetail(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(Destination:Destination)-[:AT]->(Place:Place)").Where((Destination Destination) => Destination.DestinationID == DestinationID).
                Match("(Destination)-[:HAS*]->(Content:Content)").
                Return((destination, place, contents) => new
                {
                    destination = destination.As<Destination>(),
                    place = place.As<Place>(),
                    contents = contents.CollectAs<Content>()
                }).Results;
            Destination result = new Destination();
            foreach (var item in query)
            {
                result = item.destination;
                result.Place = item.place;
                foreach (var content in item.contents)
                {
                    result.Contents.Add(content.Data);
                }
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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);
            return true;
        }
        public bool UpdateDestination(Guid UserID, Destination Destination)
        {
            var query = Db.Cypher.OptionalMatch("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)").Where((Destination destination) => destination.DestinationID == Destination.DestinationID).AndWhere((User user)=>user.UserID == UserID).
                Set("Destination = {destination}").WithParams(new { destination = Destination }).Return(destination => destination.As<Destination>()).Results;
            return (query.First<Destination>() != null);
        }
        public void DeleteDestination(Guid UserID, Guid DestinationID)
        {
            Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[r]-()").Where((Destination Destination) => Destination.DestinationID == DestinationID).AndWhere((User User) => User.UserID == UserID).
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.DestinationID == DestinationID).Set("Activity.Status = 'DELETED'").Delete("Destination, r").ExecuteWithoutResults();

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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);            
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
        public IEnumerable<Content> GetAllContent(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(content => content.As<Content>()).Results;
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
                                                " SET Destinatino.NumberOfLike = Destination.NumberOfLike + 1 " +
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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);
        }
        public void UnlikeDestination(Guid UserID, Guid DestinationID)
        {

            Db.Cypher.Match("(Destination:Destination)-[rel:LIKED_BY]->(User:User)").Where((User User) => User.UserID == UserID).AndWhere((Destination Destination) => Destination.DestinationID == DestinationID).
                 Set("Destination.NumberOfLike = Destination.NumberOfLike - 1").Delete("rel")
                .ExecuteWithoutResults();
        }
        public IEnumerable<User> GetAllUserLiked(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:LIKED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results;
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
            CypherQuery query = new CypherQuery("MATCH (User:User), (Destination:Destination) " +
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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);
        }
        public IEnumerable<User> GetAllUserShared(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:SHARED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results;
        }
    }

    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        Destination GetDestination(Guid DestinationID);
        Destination GetDestinationDetail(Guid DestinationID);
        bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID);
        bool UpdateDestination(Guid UserID, Destination Destination);
        void DeleteDestination(Guid UserID, Guid DestinationID);
        void AddNewContent(Content Content, Guid DestinationID, Guid UserID);
        void UpdateContent(Guid UserID, Content Content);
        void DeleteContent(Guid UserID, Guid ContentID);
        IEnumerable<Content> GetAllContent(Guid DestinationID);
        void LikeDestination(Guid UserID, Guid DestinationID);
        void UnlikeDestination(Guid UserID, Guid DestinationID);
        IEnumerable<User> GetAllUserLiked(Guid DestinationID);
        void ShareDestination(Guid UserID, Guid DestinationID, String Content);
        IEnumerable<User> GetAllUserShared(Guid DestinationID);
    }

}
