using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footprints.DAL.Concrete;
using NUnit.Framework;
using Footprints.Models;
using Neo4jClient;
using Footprints.Tests.DITest;
namespace Footprints.DAL.Concrete.Tests
{
    [TestFixture()]
    public class CommentRepositoryTests
    {
        BaseTestClass baseTestClass = new BaseTestClass();
        [Test()]
        public void GetAllCommentOnJourneyTest()
        {
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid JourneyID = new Guid("dcd7f7ca-5d4d-41ad-9c8a-172f395062ac");
            IList<Comment> result = baseTestClass.commentRepo.GetAllCommentOnJourney(JourneyID);
            Assert.AreEqual(result.Count, 1);
        }
    }
}
