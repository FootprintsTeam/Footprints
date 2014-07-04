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
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.userID == userID).Return(user => user.As<User>());
            return query.Results.First<User>();
        }

        public bool addNewUser(User user)
        {
            var query = Db.Cypher.CreateUnique("(user:User {user})").WithParams(new { user }).Return(userReturned => userReturned.As<User>());
            return (query.Results.First<User>() != null);
        }

        public bool addFriendRelationship(Guid userID_A, Guid userID_B)
        {
            //Cypher Query
            //MATCH (userA:User),(userB:User)
            //WHERE (userA.userID = '1') AND (userB.userID = '2')
            //CREATE (userA)-[:FRIEND]->(userB)
            //CREATE (userB)-[:FRIEND]->(userA)
            //CREATE (activityOfA:Activity { type : "ADD_NEW_FRIEND", userID : '2' ,timestamp : '03/07/2014'})
            //CREATE (activityOfB:Activity { type : "ADD_NEW_FRIEND", userID : '1' ,timestamp : '03/07/2014'})
            //WITH userA, userB, activityOfA, activityOfB
            //OPTIONAL MATCH (userA)-[f:FIRST]->(nextActivityA)
            //CREATE (userA)-[:FIRST]->(activityOfA)
            //WITH f, activityOfA, nextActivityA, userB, activityOfB
            //WHERE f IS NOT NULL
            //DELETE f
            //CREATE (activityOfA)-[:NEXT]->(nextActivityA)
            //WITH userB, activityOfB
            //OPTIONAL MATCH (userB)-[fi:FIRST]->(nextActivityB)
            //CREATE (userB)-[:FIRST]->(activityOfB)
            //WITH fi, activityOfB, nextActivityB
            //WHERE fi IS NOT NULL
            //DELETE fi
            //CREATE (activityOfB)-[:NEXT]->(nextActivityB)
            Activity activityOfA = new Activity{
                type = "ADD_FRIEND",
                userID = userID_B,
                timeStamp = DateTime.Today
            };
            Activity activityOfB = new Activity{
                type = "ADD_FRIEND",
                userID = userID_A,
                timeStamp = DateTime.Today
            };
           Db.Cypher.Match("(userA:User), (userB:User)").Where((User userA) => userA.userID == userID_A).
                                    AndWhere((User userB) => userB.userID == userID_B).
                                    Create("(userA)-[:FRIEND]->(userB)").
                                    Create("(userB)-[:FRIEND]->(userA)").
                                    Create("(activityOfA:Activity {activityOfA})").WithParams(new { activityOfA }).
                                    Create("(activityOfA:Activity {activityOfB})").WithParams(new { activityOfB }).
                                    With("userA, userB, activityOfA, activityOfB").
                                    OptionalMatch("(userA)-[f:FIRST]->(nextActivityA)").
                                    Create("(userA)-[:FIRST]->(activityOfA)").
                                    With("f, activityOfA, nextActivityA, userB, activityOfB").
                                    Where("f IS NOT NULL").
                                    Delete("f").
                                    Create("(activityOfA)-[:NEXT]->(nextActivityA)").
                                    With("userB, activityOfB").
                                    OptionalMatch("(userB)-[fi:FIRST]->(nextActivityB)").
                                    Create("(userB)-[:FIRST]->(activityOfB)").
                                    With("fi, activityOfB, nextActivityB").
                                    Where("fi IS NOT NULL").
                                    Delete("fi").
                                    Create("(activityOfB)-[:NEXT]->(nextActivityB)").ExecuteWithoutResults();
           return true;
        }

    }
    

    public interface IUserRepository : IRepository<User>
    {
        public User getUserByUserID(Guid userID);

        public bool addNewUser(User user);

        public bool addFriendRelationship(Guid userID_A, Guid userID_B)
    }
    
}
