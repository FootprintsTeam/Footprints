﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.Service
{
    public interface ICommentService { }
    public class CommentService : ICommentService
    {
        ICommentRepository _commentRepo;
        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public List<Comment> getCommentByDestinationID(Guid destinationID)
        {
            return _commentRepo.getCommentByDestinationID(destinationID);
        }

        public List<Comment> getCommentByJourneyID(Guid journeyID)
        {
            return _commentRepo.getCommentByJourneyID(journeyID);
        }

        public bool addNewCommentOnDestination(Guid userID, Comment comment)
        {
            return _commentRepo.addNewCommentOnDestination(userID, comment);
        }

        public bool addNewCommentOnJourney(Guid userID, Comment comment)
        {
            return _commentRepo.addNewCommentOnJourney(userID, comment);
        }
        public Comment getCommentByCommentID(Guid commentID) 
        {
            return _commentRepo.getCommentByCommentID(commentID);
        }
        public bool updateAComment(Comment comment)
        {
            return _commentRepo.updateAComment(comment);
        }
    }
}