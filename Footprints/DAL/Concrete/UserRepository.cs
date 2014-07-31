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
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == UserID).Return(user => user.As<User>());
            return query.Results.First<User>();
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
                ActivityID = new Guid(Guid.NewGuid().ToString("N")),
                Type = "JOIN_FOOTPRINTS",
                Timestamp = DateTimeOffset.Now
            };
            Db.Cypher.Create("(User:User {User})").WithParam("User", UserPara).
                      Create("(Activity:Activity {Activity})").WithParam("Activity", activity).With("User, Activity").
                      Match("(UserTemp:User {UserID : 'TEMP'})").
                      Create("(User)-[:EGO {UserID : User.UserID}]->(UserTemp)").Create("(User)-[:LATEST_ACTIVITY]->(Activity)")
                      .ExecuteWithoutResults();
        }
        public bool UpdateUser(User User)
        {
            var query = Db.Cypher.Match("(userTaken:User)").Where((User userTaken) => userTaken.UserID == User.UserID).
                        Set("userTaken = {user}").WithParam("user",User).Return(userTaken => userTaken.As<User>()).Results;
            return (query.First<User>() != null);
        }
        public bool AddFriendRelationship(Guid UserID_A, Guid UserID_B)
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
                ActivityID = new Guid(Guid.NewGuid().ToString("N")),
                Type = "ADD_NEW_FRIEND",
                UserID = UserID_B,
                Timestamp = DateTimeOffset.Now
            };
            Activity ActivityOfB = new Activity
            {
                ActivityID = new Guid(Guid.NewGuid().ToString("N")),
                Type = "ADD_NEW_FRIEND",
                UserID = UserID_A,
                Timestamp = DateTimeOffset.Now
            };
            String EgoEdgeOfUserA = UserID_A.ToString("N");
            String EgoEdgeOfUserB = UserID_B.ToString("N");
            Db.Cypher.Match("(userA:User), (userB:User)").Where((User userA) => userA.UserID == UserID_A).
                                     AndWhere((User userB) => userB.UserID == UserID_B).
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
                                     Create("(userB)-[:EGO {UserID : {egoB}}]->(userA)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     Create("(userA)-[:EGO {UserID : {egoB}}]->(EgoNodeOfB)").WithParams(new { egoB = EgoEdgeOfUserB }).               
                                     ExecuteWithoutResults();
            return true;
        }
        //TODO
        public bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B)
        {
            Db.Cypher.Match("(UserA:User)-[rel:FRIEND]-(UserB:User)").Where((User userA) => userA.UserID == UserID_A).
                                     AndWhere((User userB) => userB.UserID == UserID_B).Delete("rel").ExecuteWithoutResults();
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
        public IEnumerable<Report> GetReport()
        {
            return Db.Cypher.Match("report:Report").Return(report => report.As<Report>()).Results;
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
        public IEnumerable<User> GetUser()
        {
            return Db.Cypher.Match("User:User").Return(user => user.As<User>()).Results;
        }
        public IEnumerable<User> GetFriendList(Guid UserID)
        {
            return Db.Cypher.Match("(User:User)-[:FRIEND]->(Friend:User)").Where((User user) => user.UserID == UserID).Return(Friend => Friend.As<User>()).Results;
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
            var query = Db.Cypher.Match("(User:User)-[:FRIEND]->(Friend:User)").Where((User User) => User.UserID == UserID).
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
    }

    public interface IUserRepository : IRepository<User>
    {
        IEnumerable<User> GetUser();
        User GetUserByUserID(Guid UserID);
        void AddNewUser(User User);
        bool AddFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool UpdateUser(User User);
        bool BanUser(Guid UserID);
        bool ReportUser(Report Report);
        IEnumerable<Report> GetReport();
        bool UnbanUser(Guid UserID);
        bool UnactiveUser(Guid UserID);
        bool GrantAdminPrivilege(Guid UserID);
        void DeleteAnActivity(Guid ActivityID);
        IEnumerable<User> GetFriendList(Guid UserID);
        long GetNumberOfJourney(Guid UserID);
        long GetNumberOfDestination(Guid UserID);
        long GetNumberOfFriend(Guid UserID);
    }
}
