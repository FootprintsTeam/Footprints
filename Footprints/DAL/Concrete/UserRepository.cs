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

        public User GetUserByUserID(Guid userID)
        {
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Return(user => user.As<User>());
            return query.Results.First<User>();
        }

        public void AddNewUser(User userPara)
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
                Type = "JOIN_FOOTPRINTS",
                Timestamp = DateTimeOffset.Now
            };
            Db.Cypher.Create("(User:User {User})").WithParam("User", userPara).
                      Create("Activity:Activity {Activity}").WithParam("Activity", activity).With("User, Activity").
                      Match("(UserTemp:User {UserID : 'TEMP'})").
                      Create("CREATE (User)-[:EGO {UserID : User.UserID}]->(UserTemp)").Create("CREATE (User)-[:LATEST_ACTIVITY]->(Activity)")
                      .ExecuteWithoutResults();
        }

        public bool UpdateUser(User user)
        {
            var query = Db.Cypher.Match("(userTaken:User)").Where((User userTaken) => userTaken.UserID == user.UserID).
                        Set("userTaken = {user}").WithParam("user",user).Return(userTaken => userTaken.As<User>()).Results;
            return (query.First<User>() != null);
        }

        public bool AddFriendRelationship(Guid userID_A, Guid userID_B)
        {
            //Cypher Query
            //MATCH (userA:User {UserID : '1'}),(userB:User {UserID : '13'})
            //CREATE (userA)-[:FRIEND]->(userB)
            //CREATE (userB)-[:FRIEND]->(userA)
            //CREATE (activityOfA:Activity { type : "ADD_NEW_FRIEND", userID : '13' ,timestamp : '03/07/2014'})
            //CREATE (activityOfB:Activity { type : "ADD_NEW_FRIEND", userID : '1' ,timestamp : '03/07/2014'})
            //WITH userA, userB, activityOfA, activityOfB
            //MATCH (userA)-[f:LATEST_ACTIVITY]->(nextActivityA)
            //    DELETE f
            //    CREATE (userA)-[:LATEST_ACTIVITY]->(activityOfA)
            //    CREATE (activityOfA)-[:NEXT]->(nextActivityA)
            //WITH userA, userB, activityOfA, activityOfB
            //MATCH (userB)-[fi:LATEST_ACTIVITY]->(nextActivityB)
            //    DELETE fi
            //    CREATE (userB)-[:LATEST_ACTIVITY]->(activityOfB)
            //    CREATE (activityOfB)-[:NEXT]->(nextActivityB)
            //WITH userA, userB 
            //MATCH (userA)-[egoA:EGO {UserID : '1'}]->(EgoNodeOfA)
            //    DELETE egoA
            //    CREATE (userA)-[:EGO {UserID : '1'}]->(userB)
            //    CREATE (userB)-[:EGO {UserID : '1'}]->(EgoNodeOfA)
            //WITH userA, userB
            //MATCH (userB)-[egoB:EGO {UserID : '13'}]->(EgoNodeOfB)
            //    DELETE egoB
            //    CREATE (userB)-[:EGO {UserID : '13'}]->(userA)
            //    CREATE (userA)-[:EGO {UserID : '13'}]->(EgoNodeOfB)
            Activity ActivityOfA = new Activity
            {
                Type = "ADD_NEW_FRIEND",
                UserID = userID_B,
                Timestamp = DateTimeOffset.Now
            };
            Activity ActivityOfB = new Activity
            {
                Type = "ADD_NEW_FRIEND",
                UserID = userID_A,
                Timestamp = DateTimeOffset.Now
            };
            String EgoEdgeOfUserA = userID_A.ToString("N");
            String EgoEdgeOfUserB = userID_B.ToString("N");
            Db.Cypher.Match("(userA:User), (userB:User)").Where((User userA) => userA.UserID == userID_A).
                                     AndWhere((User userB) => userB.UserID == userID_B).
                                     Create("(userA)-[:FRIEND]->(userB)").
                                     Create("(userB)-[:FRIEND]->(userA)").
                                     Create("(activityOfA:Activity {activityOfA})").WithParams(new { activityOfA = ActivityOfA }).
                                     Create("(activityOfA:Activity {activityOfB})").WithParams(new { activityOfB = ActivityOfB }).                                                                         
                                     With("userA, userB, activityOfA, activityOfB").
                                     Match("(userA)-[f:LATEST_ACTIVITY]->(nextActivityA)").
                                     Delete("f").
                                     Create("(userA)-[:LATEST_ACTIVITY]->(activityOfA)").
                                     Create("(activityOfA)-[:NEXT]->(nextActivityA)").
                                     With("userA, userB, activityOfA, activityOfB").                                                                       
                                     Match("(userB)-[fi:LATEST_ACTIVITY]->(nextActivityB)").
                                     Delete("fi").
                                     Create("(userB)-[:LATEST_ACTIVITY]->(activityOfB)").
                                     Create("(activityOfB)-[:NEXT]->(nextActivityB)").
                                     With("userA, userB").
                                     Match("(userA)-[egoA:EGO {UserID : {egoA} }]->(EgoNodeOfA)").WithParams(new {egoA = EgoEdgeOfUserA}).
                                     Delete("egoA").
                                     Create("(userA)-[:EGO {UserID : {egoA}}]->(userB)").WithParams(new { egoA = EgoEdgeOfUserA }).
                                     Create("(userB)-[:EGO {UserID : {egoA}}]->(EgoNodeOfA)").WithParams(new { egoA = EgoEdgeOfUserA }).         
                                     With("userA, userB").
                                     OptionalMatch("(userB)-[egoB:EGO {UserID : {egoB}}]->(EgoNodeOfB)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     Delete("egoB").
                                     Create("(userB)-[:{egoB}]->(userA)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     Create("(userA)-[:{egoB}]->(EgoNodeOfB)").WithParams(new { egoB = EgoEdgeOfUserB }).               
                                     ExecuteWithoutResults();
            return true;
        }

        public bool DeleteFriendRelationship(Guid userID_A, Guid userID_B)
        {
            Db.Cypher.Match("(UserA:User)-[rel:FRIEND]-(UserB:User)").Where((User userA) => userA.UserID == userID_A).
                                     AndWhere((User userB) => userB.UserID == userID_B).Delete("rel").ExecuteWithoutResults();
            return true;
        }

        public bool BanUser(Guid userID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Set("user.Status = 'Baned'");
            return true;
        }

        public bool ReportUser(Report report)
        {
            Db.Cypher.Create("(report:Report {report})").WithParams(new { report = report}).ExecuteWithoutResults();
            return true;
        }

        public IEnumerable<Report> GetReport()
        {
            return Db.Cypher.Match("report:Report").Return(report => report.As<Report>()).Results;
        }
        public bool UnbanUser(Guid userID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Set("user.Status = 'Active'");
            return true;
        }

        public bool UnactiveUser(Guid userID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Set("user.Status = 'Inactive'");
            return true;
        }

        public bool GrantAdminPrivilege(Guid userID)
        {
            Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Set("user.Status = 'Admin'");
            return true;
        }

        public IEnumerable<User> GetUser()
        {
            return Db.Cypher.Match("User:User").Return(user => user.As<User>()).Results;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        IEnumerable<User> GetUser();
        User GetUserByUserID(Guid userID);
        //bool addNewUser(User user);
        void AddNewUser(User user);
        bool AddFriendRelationship(Guid userID_A, Guid userID_B);
        bool DeleteFriendRelationship(Guid userID_A, Guid userID_B);
        bool UpdateUser(User user);
        bool BanUser(Guid userID);
        bool ReportUser(Guid reporterID, Guid reporteeID);
        bool UnbanUser(Guid userID);
        bool UnactiveUser(Guid userID);
        bool GrantAdminPrivilege(Guid userID);
    }

}
