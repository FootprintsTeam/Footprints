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
                Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>();
        }
        //TODO
        public Journey GetJourneyDetail(Guid JourneyID)
        {
            Journey result = new Journey();
            var query = Db.Cypher.OptionalMatch("(Journey:Journey)").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                                  OptionalMatch("(Journey)-[:HAS*]->(Destination:Destination)").
                                  OptionalMatch("(Destination)-[:AT]->(Place:Place)").
                                  OptionalMatch("(Destination)-[:HAS*]->(Content:Content)").Return((Journey, Destination, Place, Content) => new
                                  {
                                      journey = Journey.As<Journey>(),
                                      destination = Destination.As<Destination>(),
                                      place = Place.As<Place>(),
                                      content = Content.As<Content>()
                                  }
                                  ).Results;
            Destination currentDestination = new Destination();
            Guid defaultGuid = new Guid();
            bool first = true;
            foreach (var item in query)
            {
                if (first) 
                {
                    if (!(item.journey == null)) result = item.journey;
                    first = false;
                }
                if (currentDestination.DestinationID.Equals(defaultGuid))
                {
                    if (!(item.destination == null)) currentDestination = item.destination;
                    if (!(item.place == null)) currentDestination.Place = item.place;
                    if (!(item.content == null)) currentDestination.Contents.Add(item.content);
                }
                else if (currentDestination.DestinationID == item.destination.DestinationID)
                {
                    if (!(item.content == null)) currentDestination.Contents.Add(item.content);
                }
                else
                {
                    if (!(currentDestination.DestinationID == null)) result.Destinations.Add(currentDestination);
                    if (!(item.destination == null)) currentDestination = item.destination;
                    if (!(item.place == null)) currentDestination.Place = item.place;
                    if (!(item.content == null)) currentDestination.Contents.Add(item.content);
                }
            }
            return result;
        }
        public bool AddNewJourney(Guid UserID, Journey Journey)
        {
            //Cypher Query 
            //CREATE (journey:Journey {journeyID : '10', name : 'Hanoi', description : 'Hanoi', takenDate : '17/07/2014', timestamp : '17/07/2014', numberOfLikes : '0'})
            //WITH journey
            //MATCH (user:User {UserID : '1'})
            //CREATE (user)-[:HAS_JOURNEY]->(journey)
            //CREATE (activity:Activity { type : 'CREATE_NEW_JOURNEY', journeyID : '2', timestamp : '03/07/2014'})
            //WITH user, journey, activity
            //MATCH (user)-[f:LATEST_ACTIVITY]->(nextActivity)
            //DELETE f
            //CREATE (user)-[:LATEST_ACTIVITY]->(activity)
            //CREATE (activity)-[:NEXT]->(nextActivity)
            //CREATE (activity)-[:ACT_ON_JOURNEY]->(journey)
            //WITH user
            //MATCH (user)-[:FRIEND]->(friend)
            //WITH user, COLLECT(friend) AS friends
            //UNWIND friends AS fr
            //MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(user)-[r2:EGO {UserID : fr.UserID}]->(nextUser)
            //WITH fr, user, rel, previousUser, r1, r2, nextUser, NextFriendInEgo
            //WHERE NextFriendInEgo <>  user
            //CREATE (fr)-[:EGO {UserID : fr.UserID }]->(user)
            //CREATE (user)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //WITH fr, previousUser, nextUser
            //WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL
            //CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)
            Activity activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "ADD_NEW_JOURNEY",
                UserID = UserID,
                JourneyID = Journey.JourneyID,
                Timestamp = DateTimeOffset.Now
            };
            // Neo4jClient currently doestn't support UNWIND statement
            //Db.Cypher.Create("(journey:Journey {j} )").WithParam("j", journey).With("journey").
            //        Match("(user:User)").Where((User user) => user.UserID == userID).
            //        Create("(user)-[:HAS_JOURNEY]->(journey)").
            //        Create("(activity:Activity {a})").WithParam("a", activity).
            //        With("user, journey, activity").
            //        Match("(user)-[f:LATEST_ACTIVITY]->(nextActivity)").
            //        Delete("f").
            //        Create("(user)-[:LATEST_ACTIVITY]->(activity)").
            //        Create("(activity)-[:NEXT]->(nextActivity)").
            //        Create("(activity)-[:ACT_ON_JOURNEY]->(journey)").
            //        With("user").
            //        Match("(user)-[:FRIEND]->(friend)").
            //        With("user, COLLECT(friend) AS friends").                    
            //        .ExecuteWithoutResults();
            CypherQuery query = new CypherQuery("CREATE (journey:Journey {journey}) " +
                                                " WITH journey " +
                                                " MATCH (user:User {UserID : {UserID}}) " +
                                                " CREATE (user)-[:HAS]->(journey) " +
                                                " CREATE (activity:Activity {activity}) " +
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
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser) ", new Dictionary<String, Object> { { "journey", Journey }, { "activity", activity }, {"UserID", UserID} }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteGetCypherResults<User>(query);
            return true;
        }
        public bool UpdateJourney(Guid UserID, Journey Journey)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)").Where((Journey journey) => journey.JourneyID == Journey.JourneyID).AndWhere((User user) => user.UserID == UserID).
                         Set("Journey = {journey}").WithParam("journey", Journey).Return(journey => journey.As<Journey>()).Results;
            return (query.First<Journey>() != null);
        }
        public bool DeleteJourney(Guid UserID, Guid JourneyID)
        {
            Db.Cypher.OptionalMatch("(User:User)").Where((User User) => User.UserID == UserID).
                      OptionalMatch("(User)-[rel:HAS]->(Journey:Journey)-[r]-()").Where((Journey Journey) => Journey.JourneyID == JourneyID).
                      OptionalMatch("(Activity:Activity)").Where((Activity Activity) => Activity.JourneyID == JourneyID).
                      Set("Activity.Status = 'DELETED'").
                      With("User, rel, r, Journey").
                      Where("rel IS NOT NULL").Delete("rel, r, Journey").ExecuteWithoutResults();
            return true;
        }
        public IEnumerable<Journey> GetJourneyList()
        {
            return Db.Cypher.Match("(journey:Journey)").Return(journey => journey.As<Journey>()).Results;
        }
        public IEnumerable<Journey> GetJourneyListBelongToUser(Guid UserID)
        {
            return Db.Cypher.Match("(User:User)-[:HAS]->(journey:Journey)").Where((User User) => User.UserID == UserID).Return(journey => journey.As<Journey>()).Results;
        }
        public void LikeJourney(Guid UserID, Guid JourneyID)
        {
            // Cypher Query
            //MATCH (User:User), (Journey:Journey)
            //WHERE (User.UserID = '1') AND (Journey.JourneyID = '5')
            //CREATE (Journey)-[:LIKED_BY]->(User)
            //SET Journey.numberOfLikes = Journey.numberOfLikes + 1
            //CREATE (Activity:Activity {Type : 'LIKE_A_JOURNEY', timestamp : '21/07/2014', JourneyID : '5'})
            //WITH User, Journey, Activity
            //MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity)
            //DELETE f
            //CREATE (User)-[:LATEST_ACTIVITY]->(Activity)
            //CREATE (Activity)-[:NEXT]->(nextActivity)
            //CREATE (Activity)-[:ACT_ON_JOURNEY]->(Journey)
            //WITH User
            //MATCH (User)-[:FRIEND]->(friend)
            //WITH User, COLLECT(friend) AS friends
            //UNWIND friends AS fr
            //MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser)
            //WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo
            //WHERE NextFriendInEgo <>  User
            //CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User)
            //CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //WITH fr, previousUser, nextUser
            //WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL
            //CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)

            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "LIKE_A_JOURNEY",
                JourneyID = JourneyID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:LIKED_BY]->(User) " +
                                                " SET Journey.NumberOfLike = Journey.NumberOfLike + 1 " +
                                                " CREATE (Activity:Activity {Activity}) " +
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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);
            // return true;
        }

        public void UnlikeJourney(Guid UserID, Guid JourneyID)
        {
            Db.Cypher.Match("(Journey:Journey)-[rel:LIKED_BY]->(User:User)").Where((User User) => User.UserID == UserID).AndWhere((Journey Journey) => Journey.JourneyID == JourneyID).
                 Set("Journey.NumberOfLike = Journey.NumberOfLike - 1").Delete("rel")
                .ExecuteWithoutResults();
        }
        public IEnumerable<User> GetAllUserLiked(Guid JourneyID)
        {
            return Db.Cypher.Match("(Journey:Journey)-[:LIKED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(user => user.As<User>()).Results;
        }
        public void ShareJourney(Guid UserID, Guid JourneyID, String Content)
        {
            Activity Activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "SHARE_A_JOURNEY",
                JourneyID = JourneyID,
                Content = Content,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery("MATCH (User:User), (Journey:Journey) " +
                                                " WHERE (User.UserID = {UserID} ) AND (Journey.JourneyID = {JourneyID} ) " +
                                                " CREATE (Journey)-[:SHARED_BY]->(User) " +
                                                " SET Journey.NumberOfShare = Journey.NumberOfShare + 1 " +
                                                " CREATE (Activity:Activity {Activity}) " +
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
            ((IRawGraphClient)Db).ExecuteGetCypherResults<Journey>(query);
        }
        public IEnumerable<User> GetAllUserShared(Guid JourneyID)
        {
            return Db.Cypher.Match("(Journey:Journey)-[:SHARED_BY]->(User:User)").Where((Journey Journey) => Journey.JourneyID == JourneyID).Return(user => user.As<User>()).Results;
        }
    }
    public interface IJourneyRepository : IRepository<Journey>
    {
        bool AddNewJourney(Guid UserID, Journey Journey);        
        Journey GetJourneyByID(Guid JourneyID);
        Journey GetJourneyDetail(Guid JourneyID);
        bool UpdateJourney(Guid UserID, Journey Journey);
        bool DeleteJourney(Guid UserID, Guid JourneyID);
        IEnumerable<Journey> GetJourneyList();
        IEnumerable<Journey> GetJourneyListBelongToUser(Guid UserID);
        void LikeJourney(Guid UserID, Guid JourneyID);
        void UnlikeJourney(Guid UserID, Guid JourneyID);
        IEnumerable<User> GetAllUserLiked(Guid JourneyID);
        void ShareJourney(Guid UserID, Guid JourneyID, String Content);
        IEnumerable<User> GetAllUserShared(Guid JourneyID);
    }
}
