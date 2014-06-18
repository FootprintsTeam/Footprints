using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;

namespace Footprints.Service
{
    public interface IUserService { 
    }
    public class UserService : IUserService
    {
        IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
    }
}