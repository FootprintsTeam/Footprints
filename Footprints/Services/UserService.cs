using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;

namespace Footprints.Service
{
    public interface IUserService { 
        User RetrieveUser(Guid userID);
        bool AddNewUser(User user);
        bool UpdateUser(User user);
        bool AddFriendRelationship(Guid userID_A, Guid userID_B);
    }
    public class UserService : IUserService
    {
        IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public User RetrieveUser(Guid userID)
        {
            return _userRepo.GetUserByUserID(userID);
        }

        public bool AddNewUser(User user)
        {
            //return _userRepo.addNewUser(user);
            return false;
        }

        public bool UpdateUser(User user)
        {
            return _userRepo.UpdateUser(user);
        }

        public bool AddFriendRelationship(Guid userID_A, Guid userID_B)
        {
            return _userRepo.AddFriendRelationship(userID_A, userID_B);
        }
    }
}