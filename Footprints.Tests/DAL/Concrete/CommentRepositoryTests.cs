using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footprints.DAL.Concrete;
using NUnit.Framework;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.DAL.Concrete.Tests
{
    [TestFixture()]
    public class CommentRepositoryTests
    {
        IGraphClient client = new GraphClient(new Uri("http://54.179.157.145:7474/db/data"));
        CommentRepository commentRepository;
        [Test()]
        public void GetAllCommentOnJourneyTest()
        {
            Guid JourneyID = new Guid("dcd7f7ca-5d4d-41ad-9c8a-172f395062ac");
            IList<Comment> result = commentRepository.GetAllCommentOnJourney(JourneyID);
            Assert.AreEqual(result.Count, 1);
        }
    }
}
