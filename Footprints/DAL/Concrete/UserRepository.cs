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
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IGraphClient client) : base(client) { }
        public User GetUserByUserID(Guid UserID)
        {
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Return(user => user.As<User>()).Results;
            return query.Count() == 0 ? null : query.First();
        }
        public void AddNewUser(User UserPara)
        {
            //Must set User:Status : 'ACTIVE'
            //CREATE (User:User {UserID : '2'})
            //CREATE (Activity:Activity { Type : 'JOIN_FOOTPRINTS', timestamp : '17/07/2014'})
            //WITH User, Activity
            //MATCH (UserTemp:User {UserID : 'TEMP'}) // Temporary node
            //CREATE (User)-[:EGO {UserID : User.UserID}]->(UserTemp)
            //CREATE (User)-[:LATEST_ACTIVITY]->(Activity)

            Activity activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "JOIN_FOOTPRINTS",
                Timestamp = DateTimeOffset.Now
            };
            Db.Cypher.Create("(User:User {User})").WithParam("User", UserPara).
                      Create("(Activity:Activity {Activity})").WithParam("Activity", activity).With("User, Activity").
                      Match("(UserTemp:User {UserID : 'TEMP'})").
                      Create("(User)-[:EGO {UserID : User.UserID}]->(UserTemp)").
                      Create("(User)-[:LATEST_ACTIVITY]->(Activity)").
                      Create("(User)-[:FRIEND]->(UserTemp)")
                      .ExecuteWithoutResults();
        }
        public bool UpdateUser(User User)
        {
            var query = Db.Cypher.Match("(userTaken:User)").Where((User userTaken) => userTaken.UserID == User.UserID).
                        Set("userTaken = {user}").WithParam("user",User).Return(userTaken => userTaken.As<User>()).Results;
            return query.Count() > 0 ? true : false;
        }
        public bool AddFriendRelationship(Guid UserID_A, Guid UserID_B)
        {            
            Activity ActivityOfA = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "ADD_NEW_FRIEND",
                UserID = UserID_B,
                Timestamp = DateTimeOffset.Now
            };
            Activity ActivityOfB = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "ADD_NEW_FRIEND",
                UserID = UserID_A,
                Timestamp = DateTimeOffset.Now
            };            
            CypherQuery query = new CypherQuery(" MATCH (userA:User {UserID : {UserID_A}}),(userB:User {UserID : {UserID_B}})" +
                                                " CREATE (userA)-[:FRIEND]->(userB) " + 
                                                " CREATE (userB)-[:FRIEND]->(userA) " +
                                                " CREATE (activityOfA:Activity {ActivityOfA}) " +
                                                " CREATE (activityOfB:Activity {ActivityOfB}) " + 
                                                " WITH userA, userB, activityOfA, activityOfB " +
                                                " MATCH (userA)-[f:LATEST_ACTIVITY]->(nextActivityA) " +
                                                "    DELETE f " +
                                                "    CREATE (userA)-[:LATEST_ACTIVITY]->(activityOfA) " +
                                                "    CREATE (activityOfA)-[:NEXT]->(nextActivityA) " +
                                                " WITH userA, userB, activityOfA, activityOfB " +
                                                " MATCH (userB)-[fi:LATEST_ACTIVITY]->(nextActivityB) " +
                                                "    DELETE fi " +
                                                "    CREATE (userB)-[:LATEST_ACTIVITY]->(activityOfB) " +
                                                "    CREATE (activityOfB)-[:NEXT]->(nextActivityB) " +
                                                " WITH userA, userB " +
                                                " MATCH (userA)-[egoA:EGO {UserID : {UserID_A}}]->(EgoNodeOfA) " +
                                                "    DELETE egoA " +
                                                "    CREATE (userA)-[:EGO {UserID : {UserID_A}}]->(userB) " +
                                                "    CREATE (userB)-[:EGO {UserID : {UserID_A}}]->(EgoNodeOfA) " +
                                                " WITH userA, userB " +
                                                " MATCH (userB)-[egoB:EGO {UserID : {UserID_B}}]->(EgoNodeOfB) " +
                                                "    DELETE egoB " +
                                                "    CREATE (userB)-[:EGO {UserID : {UserID_B}}]->(userA) " +
                                                "    CREATE (userA)-[:EGO {UserID : {UserID_B}}]->(EgoNodeOfB)",
                                                new Dictionary<String, Object> { { "UserID_A", UserID_A }, { "UserID_B", UserID_B }, { "ActivityOfA", ActivityOfA }, { "ActivityOfB", ActivityOfB } }, 
                                                CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }
        //TODO
        public bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B)
        {
            CypherQuery query = new CypherQuery(" MATCH (UserA:User)-[rel:FRIEND]-(UserB:User) " +
                                            " WHERE (UserA.UserID = {UserID_A}) AND (UserB.UserID = {UserID_B}) " +
                                            " MATCH (previousB)-[relPB:EGO {UserID : UserA.UserID}]->(UserB)-[relNB:EGO {UserID : UserA.UserID}]->(nextB) " +
                                            " MATCH (previousA)-[relPA:EGO {UserID : UserB.UserID}]->(UserB)-[relNA:EGO {UserID : UserB.UserID}]->(nextA) " +
                                            " DElETE rel, relPA, relPB, relNA, relNB " +
                                            " CREATE (previousA)-[:EGO {UserID : UserB.UserID}]->(nextA) " +
                                            " CREATE (previousB)-[:EGO {UserID : UserA.UserID}]->(nextB) " +
                                            " WITH UserA, UserB " +
                                            " OPTIONAL MATCH (UserA)-[:LATEST_ACTIVITY]->(LatestActivityA) " +
                                            " OPTIONAL MATCH (LatestActivityA)-[:NEXT*]->(NextActivityA) " +
                                            " WITH UserA, UserB, LatestActivityA, NextActivityA " +
                                            " WHERE (LatestActivityA.UserID = UserB.UserID) " +
                                            " SET LatestActivityA.Status = 'DELETED' " +
                                            " WITH UserA, UserB, NextActivityA " +
                                            " WHERE (NextActivityA.UserID = UserB.UserID) " +
                                            " SET NextActivityA.Status = 'DELETED' " +
                                            " WITH UserA, UserB " +
                                            " OPTIONAL MATCH (UserB)-[:LATEST_ACTIVITY]->(LatestActivityB) " +
                                            " OPTIONAL MATCH (LatestActivityB)-[:NEXT*]->(NextActivityB) " +
                                            " WITH UserA, LatestActivityB, NextActivityB " +
                                            " WHERE (LatestActivityB.UserID = UserA.UserID) " +
                                            " SET LatestActivityB.Status = 'DELETED' " +
                                            " WITH UserA, NextActivityB " +
                                            " WHERE (NextActivityB.UserID = UserA.UserID) " +
                                            " SET NextActivityB.Status = 'DELETED'", 
                new Dictionary<String, Object> { {"UserID_A", UserID_A}, {"UserID_B", UserID_B} }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }
        public bool BanUser(Guid UserID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Set("user.Status = 'Baned'");
            return true;
        }
        public bool ReportUser(Report Report)
        {
            Db.Cypher.Create("(report:Report {report})").WithParams(new { report = Report}).ExecuteWithoutResults();
            return true;
        }
        public IList<Report> GetReport()
        {
            return Db.Cypher.Match("report:Report").Return(report => report.As<Report>()).Results.ToList<Report>();
        }
        public bool UnbanUser(Guid UserID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Set("user.Status = 'Active'");
            return true;
        }
        public bool UnactiveUser(Guid UserID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Set("user.Status = 'Inactive'");
            return true;
        }
        public bool GrantAdminPrivilege(Guid UserID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Set("user.Status = 'Admin'");
            return true;
        }
        public IList<User> GetUser()
        {

            return Db.Cypher.Match("(user:User)").Where("user.UserID <> 'TEMP'").Return(user => user.As<User>()).Results.ToList<User>();
        }
        public IList<User> GetFriendList(Guid UserID)
        {
            return Db.Cypher.Match("(User:User)-[:FRIEND]-(Friend:User)").Where((User user) => user.UserID == UserID).Return(Friend => Friend.As<User>()).Results.ToList<User>();
        }
        public void DeleteAnActivity(Guid ActivityID)
        {
            Db.Cypher.Match("(Activity:Activity)").Where((Activity Activity) => Activity.ActivityID == ActivityID).Delete("Activity").ExecuteWithoutResults();
        }
        public long GetNumberOfJourney(Guid UserID)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)").Where((User User) => User.UserID == UserID).
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
        public long GetNumberOfDestination(Guid UserID)
        {
            var query = Db.Cypher.Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)").
                        Where((User User) => User.UserID == UserID).
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
        public long GetNumberOfFriend(Guid UserID)
        {
            var query = Db.Cypher.Match("(User:User)-[:FRIEND]-(Friend:User)").Where((User User) => User.UserID == UserID).
                        Return((Friend) => new
                        {
                            NumberOfFriend = Friend.Count()
                        }).Results;
            foreach (var item in query)
            {
                return item.NumberOfFriend;
            }
            return 0;
        }
        //For Admin
        public bool DeleteUser(Guid UserID)
        {
            CypherQuery query = new CypherQuery(" OPTIONAL MATCH (User:User)-[r]-() WHERE (User.UserID = {UserID}) AND (User.Status <> 'Admin') " +
                                                " OPTIONAL MATCH (User)-[:LATEST_ACTIVITY]->(LatestActivity:Activity) " +
                                                " OPTIONAL MATCH (LatestActivity)-[:NEXT*]->(NextActivity:Activity) " +
                                                " OPTIONAL MATCH (User)-[:FRIEND]-(friend:User) " +
                                                " WITH User, r, LatestActivity, NextActivity, Collect(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " OPTIONAL MATCH (previous)-[:EGO {UserID : fr.UserID}]->(User) " +
                                                " OPTIONAL MATCH (User)-[:EGO {UserID : fr.UserID}]->(next) " +
                                                " DELETE User, r, LatestActivity, NextActivity " +
                                                " WITH previous, next, fr " +
                                                " WHERE (previous IS NOT NULL) AND (next IS NOT NULL) " +
                                                " CREATE (previous)-[:EGO {UserID : fr.UserID}]->(next) ",
                                                new Dictionary<String, Object> { {"UserID", UserID} }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }
        public bool UpdateProfilePicURL(Guid UserID, String ProfilePicURL)
        {
            var query = Db.Cypher.Match("(User:User)").Where((User User) => User.UserID == UserID).
                        Set("User.ProfilePicURL = {ProfilePicURL}").WithParam("ProfilePicURL", ProfilePicURL).Return(User => User.As<User>()).Results;
            return query.Count() > 0 ? true : false;
        }
        public bool UpdateCoverPhotoURL(Guid UserID, String CoverPhotoURL)
        {
            var query = Db.Cypher.Match("(User:User)").Where((User User) => User.UserID == UserID).
                        Set("User.CoverPhotoURL = {CoverPhotoURL}").WithParam("CoverPhotoURL", CoverPhotoURL).Return(User => User.As<User>()).Results;
            return query.Count() > 0 ? true : false;
        }
        public bool ChangePassword(Guid UserID, String Password)
        {
            var query = Db.Cypher.Match("(User:User)").Where((User User) => User.UserID == UserID).
                        Set("User.Password = {Password}").WithParam("Password", Password).Return(User => User.As<User>()).Results;
            return query.Count() > 0 ? true : false;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        IList<User> GetUser();
        User GetUserByUserID(Guid UserID);
        void AddNewUser(User User);
        bool AddFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool UpdateUser(User User);
        bool BanUser(Guid UserID);
        bool ReportUser(Report Report);
        IList<Report> GetReport();
        bool UnbanUser(Guid UserID);
        bool UnactiveUser(Guid UserID);
        bool GrantAdminPrivilege(Guid UserID);
        void DeleteAnActivity(Guid ActivityID);
        IList<User> GetFriendList(Guid UserID);
        long GetNumberOfJourney(Guid UserID);
        long GetNumberOfDestination(Guid UserID);
        long GetNumberOfFriend(Guid UserID);
        bool DeleteUser(Guid UserID);
        bool UpdateProfilePicURL(Guid UserID, String ProfilePicURL);
        bool UpdateCoverPhotoURL(Guid UserID, String CoverPhotoURL);
        bool ChangePassword(Guid UserID, String Password);
    }
}
