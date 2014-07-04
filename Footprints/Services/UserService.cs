using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;

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

        public User getUserByUserID(Guid userID)
        {
            return _userRepo.getUserByUserID(userID);
        }

        public bool addNewUser(User user)
        {
            //return _userRepo.addNewUser(user);
            return false;
        }

        public bool updateUser(User user)
        {
            return _userRepo.updateUser(user);
        }

        public bool addFriendRelationship(Guid userID_A, Guid userID_B)
        {
            return _userRepo.addFriendRelationship(userID_A, userID_B);
        }
    }
}