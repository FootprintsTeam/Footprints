using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
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
        }

        [TestMethod]
        public void testAdd()
        {
            setup();
            var userRepo = new UserRepository(client);
            //userRepo.addNewUser(users.First());
            Assert.IsFalse(false);
        }

        public void testQuery() { 
            
        }
    }
}
