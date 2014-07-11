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
    public class JourneyDALTest : BaseTestClass
    {
        [TestMethod]
        public void addNewJourney()
        {
            journeyRep.addNewJourney( users.First().userID, journeys.First());
        }

        [TestMethod]
        public void getJourneyByID() {
            var result = journeyRep.getJourneyByID(journeys.First().UserID);
            Assert.AreEqual(result.UserID, journeys.First().UserID);
        }

        [TestMethod]
        public void getNumberOfLikes() {
            var result = journeyRep.getNumberOfLikes(journeys.First().JourneyID);
            Assert.AreEqual(result, journeys.First().NumberOfLike);
        }
    }
}
