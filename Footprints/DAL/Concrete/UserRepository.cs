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

        public User getUserByUserID(Guid userID)
        {
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.UserID == userID).Return(user => user.As<User>());
            return query.Results.First<User>();
        }

        public void addNewUser(User userPara)
        {
            Db.Cypher.Create("(user:User {user})").WithParam("user", userPara).Return(user => user.As<User>()).ExecuteWithoutResults();
        }

        public bool updateUser(User user)
        {
            var query = Db.Cypher.Match("(userTaken:User)").Where((User userTaken) => userTaken.UserID == user.UserID).
                        Set("userTaken = {user}").WithParam("user",user).Return(userTaken => userTaken.As<User>()).Results;
            return (query.First<User>() != null);
        }

        public bool addFriendRelationship(Guid userID_A, Guid userID_B)
        {
            //Cypher Query
            //MATCH (userA:User),(userB:User)
            //WHERE (userA.userID = 'ad49da1f-0481-4625-b906-66fbb2152474') AND (userB.userID = '43517b3c-745f-49c9-8c1b-9e9de49dacee')
            //CREATE (userA)-[:FRIEND]->(userB)
            //CREATE (userB)-[:FRIEND]->(userA)
            //CREATE (activityOfA:Activity { type : "ADD_NEW_FRIEND", userID : '43517b3c-745f-49c9-8c1b-9e9de49dacee' ,timestamp : '03/07/2014'})
            //CREATE (activityOfB:Activity { type : "ADD_NEW_FRIEND", userID : 'ad49da1f-0481-4625-b906-66fbb2152474' ,timestamp : '03/07/2014'})
            //WITH userA, userB, activityOfA, activityOfB
            //OPTIONAL MATCH (userA)-[f:LATEST_ACTIVITY]->(nextActivityA)
            //CREATE (userA)-[:LATEST_ACTIVITY]->(activityOfA)
            //WITH userA, f, activityOfA, nextActivityA, userB, activityOfB
            //WHERE f IS NOT NULL
            //DELETE f
            //CREATE (activityOfA)-[:NEXT]->(nextActivityA)
            //WITH userA, userB, activityOfB
            //OPTIONAL MATCH (userB)-[fi:LATEST_ACTIVITY]->(nextActivityB)
            //CREATE (userB)-[:LATEST_ACTIVITY]->(activityOfB)
            //WITH userA, userB, fi, activityOfB, nextActivityB
            //WHERE fi IS NOT NULL
            //DELETE fi
            //CREATE (activityOfB)-[:NEXT]->(nextActivityB)
            //WITH userA, userB
            //OPTIONAL MATCH (userA)-[egoA:egoad49da1f04814625b90666fbb2152474]->(EgoNodeOfA)
            //CREATE (userA)-[:egoad49da1f04814625b90666fbb2152474]->(userB)
            //WITH userA, userB, egoA, EgoNodeOfA
            //WHERE egoA IS NOT NULL
            //DELETE egoA
            //CREATE (userB)-[:egoad49da1f04814625b90666fbb2152474]->(EgoNodeOfA)
            //WITH userA, userB
            //OPTIONAL MATCH (userB)-[egoB:ego43517b3c745f49c98c1b9e9de49dacee]->(EgoNodeOfB)
            //CREATE (userB)-[:ego43517b3c745f49c98c1b9e9de49dacee]->(userA)
            //WITH userA, userB, egoB, EgoNodeOfB
            //WHERE egoB IS NOT NULL
            //DELETE egoB
            //CREATE (userA)-[:ego43517b3c745f49c98c1b9e9de49dacee]->(EgoNodeOfB)
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
                                     OptionalMatch("(userA)-[f:LATEST_ACTIVITY]->(nextActivityA)").
                                     Create("(userA)-[:LATEST_ACTIVITY]->(activityOfA)").
                                     With("f, userA, activityOfA, nextActivityA, userB, activityOfB").
                                     Where("f IS NOT NULL").
                                     Delete("f").
                                     Create("(activityOfA)-[:NEXT]->(nextActivityA)").
                                     With("userA, userB, activityOfB").
                                     OptionalMatch("(userB)-[fi:LATEST_ACTIVITY]->(nextActivityB)").
                                     Create("(userB)-[:LATEST_ACTIVITY]->(activityOfB)").
                                     With("userA, userB, fi, activityOfB, nextActivityB").
                                     Where("fi IS NOT NULL").
                                     Delete("fi").
                                     Create("(activityOfB)-[:NEXT]->(nextActivityB)").
                                     With("userA, userB").
                                     OptionalMatch("(userA)-[egoA:{egoA}]->(EgoNodeOfA)").WithParams(new {egoA = EgoEdgeOfUserA}).
                                     Create("(userA)-[:{egoA}]->(userB)").WithParams(new { egoA = EgoEdgeOfUserA }).
                                     With("userA, egoA, EgoNodeOfA, userB").
                                     Where("egoA IS NOT NULL").
                                     Delete("egoA").
                                     Create("(userB)-[:{egoA}]->(EgoNodeOfA)").WithParams(new {egoA = EgoEdgeOfUserA}).
                                     With("userA, userB").
                                     OptionalMatch("(userB)-[egoB:{egoB}]->(EgoNodeOfB)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     Create("(userB)-[:{egoB}]->(userA)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     With("userA, egoB, EgoNodeOfB, userB").
                                     Where("egoB IS NOT NULL").
                                     Delete("egoB").
                                     Create("(userA)-[:{egoB}]->(EgoNodeOfB)").WithParams(new { egoB = EgoEdgeOfUserB }).
                                     ExecuteWithoutResults();
            return true;
        }

    }

    public interface IUserRepository : IRepository<User>
    {
        User getUserByUserID(Guid userID);

        //bool addNewUser(User user);
        void addNewUser(User user);
        bool addFriendRelationship(Guid userID_A, Guid userID_B);
        bool updateUser(User user);
    }

}
