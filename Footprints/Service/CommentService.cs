using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;

namespace Footprints.Service
{
    public interface ICommentService { }
    public class CommentService
    {
        ICommentRepository _commentRepo;
        public CommentService(ICommentRepository commentRepo) {
            _commentRepo = commentRepo;
        }
    }
}