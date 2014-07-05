using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
using Neo4jClient.Cypher;
using Footprints.Models;
using Footprints.DAL.Concrete;
using System.Data;
using System.Data.Entity;

namespace Footprints.Tests.DITest
{
    [TestClass]
    public class UserDALTest
    {
        static IList<User> users;
        static GraphClient client;
        static UserRepository userRep;
        static void setup()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"));
            client.Connect();
            users = new List<User>();
            users.Add(new User
            {
                userID = new Guid(),
                firstName = "Nhan",
                lastName = "Trinh"            
            });
            userRep = new UserRepository(client);
        }

        public UserDALTest() {
            setup();
        }

        [TestMethod]
        public void testAdd()
        {                        
            userRep.addNewUser(users.First());            
        }

        //[TestMethod]
        //public void testQuery() {
        //    var x = userRep.getUserByUserID(users.First<User>().userID);
        //}
        [TestMethod]
        public void testCypherQuery() {
            CypherQuery query = new CypherQuery("CREATE (ee:Person {person})", new Dictionary<string, object> { { "person", users.First() } }, CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }
    }
}
