using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.DAL.Concrete.Tests
{
    [TestClass]
    public class CommentDALTest
    {
        CommentRepository commentRepo;
        [TestMethod]
        public void GetAllCommentOnJourneyTest()
        {
            Guid JourneyID = new Guid("b35046c9-d9ef-4061-883d-e6990679fb26");
            IList<Comment> result = commentRepo.GetAllCommentOnJourney(JourneyID);
            Assert(result.Count == 0);
        }
    }
}

namespace Footprints.Tests.DITest
{
    [TestClass]
    public class CommentDALTest
    {
    }
}
