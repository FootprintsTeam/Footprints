using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class Users : RepositoryBase<User>, IUserRepository
    {
        public Users(IGraphClient client) : base(client) { }        
    }

    public interface IUserRepository : IRepository<User>
    { }
    
}
