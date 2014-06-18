using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Neo4jClient;
using Footprints.Models;
namespace Footprints.DAL.Concrete
{
    public class CommentRepository : RepositoryBase<CommentRepository>, ICommentRepository
    {
        public CommentRepository(IGraphClient client) : base(client) { }

        //TODO

        public List<Comment> getCommentByJourneyID(String journeyID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.journeyID == journeyID).Return(comment => comment.As<Comment>());
            return query.Results.ToList<Comment>();
        }

        public List<Comment> getCommentByDestinationID(String destinationID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.destinationID == destinationID).Return(comment => comment.As<Comment>());
            return query.Results.ToList<Comment>();
        }

    }

    public interface ICommentRepository : IRepository<CommentRepository>
    {

    }
}
