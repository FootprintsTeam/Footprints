using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class Comments : RepositoryBase<Comments>, ICommentRepository
    {
        public Comments(IGraphClient client) : base(client) { }
    }

    public interface ICommentRepository : IRepository<Comments> { 
    
    }
}
