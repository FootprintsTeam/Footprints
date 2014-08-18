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
            userRep.AddNewUser(users.First());            
        }

        [TestMethod]
        public void getUserByUserID()
        {
            User x = userRep.GetUserByUserID(users.First<User>().UserID);
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void updateUser() {
            var x = users.First<User>();
            x.FirstName = "Thang";
            userRep.UpdateUser(x);
            Assert.AreEqual(x.FirstName, userRep.GetUserByUserID(x.UserID).FirstName);
        }

        [TestMethod]
        public void testCypherQuery() {
            //CypherQuery query = new CypherQuery("CREATE (ee:Person {person})", new Dictionary<string, object> { { "person", users.First() } }, CypherResultMode.Set);
            CypherQuery query = new CypherQuery("match (ee:Person) return ee",null,CypherResultMode.Projection);            
        }
    }
}
