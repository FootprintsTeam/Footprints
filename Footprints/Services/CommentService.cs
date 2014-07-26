﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.Service
{
    public interface ICommentService {
        IEnumerable<Comment> RetrieveDestinationComment(Guid DestinationID);
        IEnumerable<Comment> RetrieveJourneyComment(Guid JourneyID);
        bool AddDestinationComment(Guid UserID, Comment Comment);
        bool AddJourneyComment(Guid UserID, Comment Comment);
        Comment RetrieveComment(Guid CommentID);
        bool UpdateComment(Comment Comment);
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

        public IEnumerable<Comment> RetrieveDestinationComment(Guid DestinationID)
        {
            return _commentRepo.GetAllCommentOnDestination(DestinationID);
        }

        public IEnumerable<Comment> RetrieveJourneyComment(Guid JourneyID)
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
        public bool UpdateComment(Comment Comment)
        {
            return _commentRepo.UpdateComment(Comment);
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