using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
using Footprints.Models;
using Footprints.DAL;
namespace Footprints.Tests.DITest
{
    [TestClass]
    public class JourneyDALTest
    {
        static IList<Journey> journeys;
        static void setup() {
            GraphClient client = new GraphClient(new Uri("http://localhost:7474/db/data"));
            client.Connect();
            journeys.Add(new Journey
            {
                journeyID = new Guid(),
                description = "Journey Description",
                name = "Nhan's Journey",
                numberOfLikes = 2,
                takenDate = DateTime.Now,
                timestamp = DateTime.Now
            });
        }

        [TestMethod]
        public void testAdd() { 
            
        }
    }
}
