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
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid JourneyID = new Guid("dcd7f7ca-5d4d-41ad-9c8a-172f395062ac");
            //Act
            IList<Comment> result = baseTestClass.commentRepo.GetAllCommentOnJourney(JourneyID);
            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [Test()]
        public void GetAllCommentOnDestinationTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid DestinationID = new Guid("619bff15-6fc6-4976-8d9b-10f5105f395f");
            //Act
            IList<Comment> result = baseTestClass.commentRepo.GetAllCommentOnDestination(DestinationID);
            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [Test()]
        public void GetACommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = new Guid("d8ed3960-e5f8-4120-a51c-cbc9d98befb7");
            //Act
            Comment result = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(result.Content, "Đẹp lắm (:");
        }

        [Test()]
        public void UpdateCommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = new Guid("fa8ebcd8-2073-4638-a1d8-f897b88c91a4");
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            Comment comment = new Comment
            {
                CommentID = CommentID,
                Content = "Updated Comment !!!"
            };
            //Act
            baseTestClass.commentRepo.UpdateComment(UserID, comment);
            Comment result = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(result.Content, "Updated Comment !!!");
        }

        [Test()]
        public void AddDestinationCommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = Guid.NewGuid();
            Guid DestinationID = new Guid("619bff15-6fc6-4976-8d9b-10f5105f395f");
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            Comment comment = new Comment
            {
                CommentID = CommentID,
                DestinationID = DestinationID,
                NumberOfLike = 0,
                Content = "Added to Destination from Test Method !!!"
            };
            //Act
            baseTestClass.commentRepo.AddDestinationComment(DestinationID,comment);
            Comment result = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(result.Content, "Added to Destination from Test Method !!!");
        }

        [Test()]
        public void AddJourneyCommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = Guid.NewGuid();
            Guid JourneyID = new Guid("dcd7f7ca-5d4d-41ad-9c8a-172f395062ac");
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            Comment comment = new Comment
            {
                CommentID = CommentID,
                JourneyID = JourneyID,
                NumberOfLike = 0,
                Content = "Added to Journey from Test Method !!!"
            };
            //Act
            baseTestClass.commentRepo.AddJourneyComment(JourneyID, comment);
            Comment result = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(result.Content, "Added to Journey from Test Method !!!");
        }

        [Test()]
        public void LikeACommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = new Guid("fa8ebcd8-2073-4638-a1d8-f897b88c91a4");
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            //Act
            Comment beforeLike = baseTestClass.commentRepo.GetAComment(CommentID);
            baseTestClass.commentRepo.LikeAComment(UserID,CommentID);
            Comment afterLike = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(beforeLike.NumberOfLike, afterLike.NumberOfLike - 1);
        }

        [Test()]
        public void UnlikeACommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = new Guid("fa8ebcd8-2073-4638-a1d8-f897b88c91a4");
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            //Act
            Comment beforeLike = baseTestClass.commentRepo.GetAComment(CommentID);
            baseTestClass.commentRepo.UnlikeAComment(UserID, CommentID);
            Comment afterLike = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(beforeLike.NumberOfLike, afterLike.NumberOfLike + 1);
        }

        [Test()]
        public void GetAllUserLikeCommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid CommentID = new Guid("fa8ebcd8-2073-4638-a1d8-f897b88c91a4");
            //Act
            IList<User> result = baseTestClass.commentRepo.GetAllUserLikeComment(CommentID);
            //Assert
            Assert.AreEqual(result.Count, 2);
        }

        [Test()]
        public void DeleteACommentTest()
        {
            //Assign
            baseTestClass.commentRepo = new CommentRepository(baseTestClass.client);
            Guid UserID = new Guid("4663adb6-92ad-4609-8197-43ee565a096f");
            Guid CommentID = new Guid("fa8ebcd8-2073-4638-a1d8-f897b88c91a4");
            //Act
            baseTestClass.commentRepo.DeleteAComment(UserID ,CommentID);
            Comment result = baseTestClass.commentRepo.GetAComment(CommentID);
            //Assert
            Assert.AreEqual(result, null);
        }

    }
}
