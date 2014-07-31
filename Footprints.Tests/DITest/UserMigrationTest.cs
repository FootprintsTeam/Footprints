using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footprints.Models;
using Footprints.DAL.Concrete;
using Footprints.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;

namespace Footprints.Tests.DITest
{

    [TestClass]
    public class UserMigrationTest : BaseTestClass
    {
        UserRepository userRepository;
        UserMigrator userMigrator;
        public UserMigrationTest() {
            userRepository = new UserRepository(client);
            userMigrator = new UserMigrator(userRepository);
        }

        [TestMethod]
        public void TestMigration() {
            this.userMigrator.migrate();
        }
    }
}
