using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.Service
{
    public interface ICommentService {
        List<Comment> RetrieveDestinationComment(Guid destinationID);
        List<Comment> RetrieveJourneyComment(Guid journeyID);
        bool AddDestinationComment(Guid userID, Comment comment);
        bool AddJourneyComment(Guid userID, Comment comment);
        Comment RetrieveComment(Guid commentID);
        bool UpdateComment(Comment comment);
        void LikeAComment(Guid UserID, Guid CommentID);
        void UnlikeAComment(Guid UserID, Guid CommentID);
        IEnumerable<User> GetAllUserLikeComment(Guid CommentID);
        void DeleteAComment(Guid CommentID);
    }
    public class CommentService : ICommentService
    {
        ICommentRepository _commentRepo;
        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public List<Comment> RetrieveDestinationComment(Guid destinationID)
        {
            return _commentRepo.GetAllCommentOnDestination(destinationID);
        }

        public List<Comment> RetrieveJourneyComment(Guid journeyID)
        {
            return _commentRepo.GetAllCommentOnJourney(journeyID);
        }

        public bool AddDestinationComment(Guid userID, Comment comment)
        {
            return _commentRepo.AddDestinationComment(userID, comment);
        }

        public bool AddJourneyComment(Guid userID, Comment comment)
        {
            return _commentRepo.AddJourneyComment(userID, comment);
        }
        public Comment RetrieveComment(Guid commentID)
        {
            return _commentRepo.GetAComment(commentID);
        }
        public bool UpdateComment(Comment comment)
        {
            return _commentRepo.UpdateComment(comment);
        }
        public void LikeAComment(Guid UserID, Guid CommentID)
        {
            _commentRepo.LikeAComment(UserID, CommentID);
        }
        public void UnlikeAComment(Guid UserID, Guid CommentID)
        {
            _commentRepo.UnlikeAComment(UserID, CommentID);
        }
        public IEnumerable<User> GetAllUserLikeComment(Guid CommentID)
        {
            return _commentRepo.GetAllUserLikeComment(CommentID);
        }
        public void DeleteAComment(Guid CommentID)
        {
            _commentRepo.DeleteAComment(CommentID);
        }
    }
}