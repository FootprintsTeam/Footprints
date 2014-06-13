using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class CommentRepository : RepositoryBase<CommentRepository>, ICommentRepository
    {
        public CommentRepository(IGraphClient client) : base(client) { }
    }

    public interface ICommentRepository : IRepository<CommentRepository> { 
    
    }
}
