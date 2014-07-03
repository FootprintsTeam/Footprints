using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
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
            //CREATE (activity:Activity { type : "ADD_NEW_FRIEND", timestamp : '03/07/2014'})
            //CREATE (userA)-[:GENERATE]->(activity)
            //CREATE (userB)-[:GENERATE]->(activity)
            //WITH userA, userB, activity
            //MATCH (userA)-[f:FIRST]->(nextActivityA)
            //DELETE f
            //CREATE (userA)-[:FIRST]->(activity)
            //CREATE (activity)-[:NEXT]->(nextActivityA)
            //WITH userA, userB, activity, nextActivityA
            //MATCH (userB)-[fi:FIRST]->(nextActivityB)
            //DELETE fi
            //WITH userA, userB, activity, nextActivityB
            //CREATE (userB)-[:FIRST]->(activity)
            //CREATE (activity)-[:NEXT]->(nextActivityB)
            Activity activity = new Activity{
                type = "ADD_FRIEND",
                timeStamp = DateTime.Today
            };
           Db.Cypher.Match("(userA:User), (userB:User)").Where((User userA) => userA.userID == userID_A).
                                    AndWhere((User userB) => userB.userID == userID_B).
                                    Create("(userA)-[:FRIEND]->(userB)").
                                    Create("(userB)-[:FRIEND]->(userA)").
                                    Create("(activity:Activity) {activity}").WithParams(new { activity }).
                                    Create("(userA)-[:GENERATE]->(activity)").
                                    Create("(userB)-[:GENERATE]->(activity)").
                                    With("userA, userB, activity").
                                    Match("(userA)-[f:FIRST]->(nextActivityA)").Delete("f").
                                    Create("(userA)-[:FIRST]->(activity)").
                                    Create("(activity)-[:NEXT]->(nextActivityA)").
                                    With("userA, userB, activity, nextActivityA").
                                    Match("(userB)-[fi:FIRST]->(nextActivityB)").Delete("fi").
                                    With("userA, userB, activity, nextActivityB").
                                    Create("(userB)-[:FIRST]->(activity)").
                                    Create("CREATE (activity)-[:NEXT]->(nextActivityB)").ExecuteWithoutResults();
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
