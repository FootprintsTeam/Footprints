using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.Services
{
    public interface ICommentService {
        IList<Comment> RetrieveDestinationComment(Guid DestinationID);
        IList<Comment> RetrieveJourneyComment(Guid JourneyID);
        bool AddDestinationComment(Guid UserID, Comment Comment);
        bool AddJourneyComment(Guid UserID, Comment Comment);
        Comment RetrieveComment(Guid CommentID);
        bool UpdateComment(Guid UserID, Comment Comment);
        void LikeAComment(Guid UserID, Guid CommentID);
        void UnlikeAComment(Guid UserID, Guid CommentID);
        IList<User> GetAllUserLikeComment(Guid CommentID);
        void DeleteAComment(Guid UserID, Guid CommentID);
    }
    public class CommentService : ICommentService
    {
        ICommentRepository _commentRepo;
        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public IList<Comment> RetrieveDestinationComment(Guid DestinationID)
        {
            return _commentRepo.GetAllCommentOnDestination(DestinationID);
        }

        public IList<Comment> RetrieveJourneyComment(Guid JourneyID)
        {
            return _commentRepo.GetAllCommentOnJourney(JourneyID);
        }

        public bool AddDestinationComment(Guid userID, Comment Comment)
        {
            return _commentRepo.AddDestinationComment(userID, Comment);
        }

        public bool AddJourneyComment(Guid UserID, Comment Comment)
        {
            return _commentRepo.AddJourneyComment(UserID, Comment);
        }
        public Comment RetrieveComment(Guid CommentID)
        {
            return _commentRepo.GetAComment(CommentID);
        }
        public bool UpdateComment(Guid UserID, Comment Comment)
        {
            return _commentRepo.UpdateComment(UserID, Comment);
        }
        public void LikeAComment(Guid UserID, Guid CommentID)
        {
            _commentRepo.LikeAComment(UserID, CommentID);
        }
        public void UnlikeAComment(Guid UserID, Guid CommentID)
        {
            _commentRepo.UnlikeAComment(UserID, CommentID);
        }
        public IList<User> GetAllUserLikeComment(Guid CommentID)
        {
            return _commentRepo.GetAllUserLikeComment(CommentID);
        }
        public void DeleteAComment(Guid UserID, Guid CommentID)
        {
            _commentRepo.DeleteAComment(UserID, CommentID);
        }
    }
}