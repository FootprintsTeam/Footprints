using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IGraphClient client) : base(client) { }

        public User getUserByUserID(String userID)
        {
            var query = Db.Cypher.Match("(user:User)").Where((User user) => user.userID == userID).Return(user => user.As<User>());
            return query.Results.First<User>();
        }

    }
    

    public interface IUserRepository : IRepository<User>
    { 
    }
    
}
