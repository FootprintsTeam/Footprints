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
    public class UserDALTest : BaseTestClass
    {        

        [TestMethod]
        public void addNewUser()
        {                        
            userRep.addNewUser(users.First());            
        }

        [TestMethod]
        public void getUserByUserID()
        {
            User x = userRep.getUserByUserID(users.First<User>().userID);
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void updateUser() {
            var x = users.First<User>();
            x.firstName = "Thang";
            userRep.updateUser(x);
            Assert.AreEqual(x.firstName, userRep.getUserByUserID(x.userID).firstName);
        }

        [TestMethod]
        public void testCypherQuery() {
            CypherQuery query = new CypherQuery("CREATE (ee:Person {person})", new Dictionary<string, object> { { "person", users.First() } }, CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(query);
        }
    }
}
