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
        public Destination GetADestination(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>();
        }

        public Destination GetADestinationDetail(Guid DestinationID)
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
        //TODO
        // Add a destination to the tail of LinkedList
        public bool AddNewDestination(Destination Destination, String PlaceID, Guid JourneyID)
        {

            return false;
        }
        //TODO
        //Add the first Destination of a Journey [:BEGIN] relationship
        public bool AddTheFirstDestination(Destination Destination, String PlaceID, Guid JourneyID)
        {
            //Cypher Query
            //MATCH (Place:Place) WHERE (Place.PlaceID = '1')
            //MATCH (Journey:Journey) WHERE (Journey.JourneyID = '1')
            //MATCH (Journey)-[rel:BEGIN]->(FirstDestination)
            //CREATE (Destination:Destination {DestinationID : '2', Name : 'Footprints 2'})
            //CREATE (Destination)-[:AT]->(Place)
            //CREATE (Journey)-[:BEGIN]->(Destination)
            //WITH Journey, rel, Destination, FirstDestination
            //WHERE rel IS NOT NULL
            //DELETE rel
            //CREATE (Destination)-[:NEXT]->(FirstDestination)
            Db.Cypher.Match("(Place:Place)").Where((Place Place) => Place.PlaceID == PlaceID).
                        Match("(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                        OptionalMatch("MATCH (Journey)-[rel:BEGIN]->(FirstDestination)").
                        Create("(Destination:Destination {Destination})").WithParam("Destination", Destination).
                        Create("(Destination)-[:AT]->(Place)").
                        Create(" (Journey)-[:BEGIN]->(Destination)").
                        With("Journey, rel, Destination, FirstDestination").
                        Where("rel IS NOT NULL").Delete("rel").
                        Create("CREATE (Destination)-[:NEXT]->(FirstDestination)").ExecuteWithoutResults();
            return true;
        }

        public bool UpdateDestination(Destination Destination)
        {
            var query = Db.Cypher.Match("(destinationTaken:Destination)").Where((Destination destinationTaken) => destinationTaken.DestinationID == Destination.DestinationID).
                Set("destinationTaken = {destination}").WithParams(new { destination = Destination }).Return(destinationReturned => destinationReturned.As<Destination>()).Results;
            return (query.First<Destination>() != null);
        }
        public void DeleteDestination(Guid DestinationID)
        {
            Db.Cypher.Match("(destinationTaken:Destinatino)-[r]-()").Where((Destination destinationTaken) => destinationTaken.DestinationID == DestinationID).
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.DestinationID == DestinationID).Set("Activity.Status = 'DELETED'").Delete("destinationTaken, r").ExecuteWithoutResults();

        }

        //TODO
        public void AddNewContent(Content Content, Guid DestinationID)
        {

        }
        public void UpdateContent(Content Content)
        {
            Db.Cypher.Match("(Content:Content)").Where((Content content) => content.ContentID == Content.ContentID).
                        Set("Content = {Content}").WithParam("Content", Content).ExecuteWithoutResults();
        }
        public void DeleteContent(Guid ContentID)
        {
            Db.Cypher.Match("(contentTaken:Journey)-[r]-()").Where((Content contentTaken) => contentTaken.ContentID == ContentID).
                Match("(Activity:Activity)").Where((Activity Activity) => Activity.ContentID == ContentID).Set("Activity.Status = 'DELETED'").Delete("journeyTaken, r").ExecuteWithoutResults();
        }
        public IEnumerable<Content> GetAllContent(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(content => content.As<Content>()).Results;
        }
        public void AddNewPlace(Place Place)
        {
            Db.Cypher.Create("(Place:Place {Place})").WithParam("Place", Place).ExecuteWithoutResults();
        }
        public int GetNumberOfLike(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>().NumberOfLike;
        }
        public void LikeDestination(Guid UserID ,Guid DestinationID) 
        {
            Activity Activity = new Activity
            {
                Type = "LIKE_A_DESTINATION",
                DestinationID = DestinationID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery("MATCH (User:User), (Destination:Destination) " +
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
                                                new Dictionary<String, Object> { { "UserID", UserID }, { "JoureyID", DestinationID }, { "Activity", Activity } }, CypherResultMode.Projection);
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

        public int GetNumberOfShare(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>().NumberOfShare;
        }

        public void ShareDestination(Guid UserID, Guid DestinationID, String Content)
        {

        }
        public IEnumerable<User> GetAllUserShared(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:SHARED_BY]->(User:User)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(user => user.As<User>()).Results;
        }
    }

    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        Destination GetADestination(Guid DestinationID);
        Destination GetADestinationDetail(Guid DestinationID);

        bool AddNewDestination(Destination Destination);

        bool UpdateDestination(Destination Destination);
        void DeleteDestination(Guid DestinationID);
        void AddNewContent(Content Content, Guid DestinationID);
        void UpdateContent(Content Content);
        void DeleteContent(Guid ContentID);
        IEnumerable<Content> GetAllContent(Guid DestinationID);
        void AddNewPlace(Place Place);
        int GetNumberOfLike(Guid DestinationID);
        void LikeDestination(Guid UserID, Guid DestinationID);
        void UnlikeDestination(Guid UserID, Guid DestinationID);
        IEnumerable<User> GetAllUserLiked(Guid DestinationID);
        int GetNumberOfShare(Guid DestinationID);
        void ShareDestination(Guid UserID, Guid DestinationID, String Content);
        IEnumerable<User> GetAllUserShared(Guid DestinationID);
    }

}
