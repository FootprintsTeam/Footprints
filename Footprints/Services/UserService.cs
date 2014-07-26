using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;

namespace Footprints.Service
{
    public interface IUserService {
        IEnumerable<User> GetUser();
        User RetrieveUser(Guid userID);
        //bool addNewUser(User user);
        void AddNewUser(User user);
        bool AddFriendRelationship(Guid userID_A, Guid userID_B);
        bool DeleteFriendRelationship(Guid userID_A, Guid userID_B);
        bool UpdateUser(User user);
        bool BanUser(Guid userID);
        bool ReportUser(Report Report);
        IEnumerable<Report> GetReport();
        bool UnbanUser(Guid userID);
        bool UnactiveUser(Guid userID);
        bool GrantAdminPrivilege(Guid userID);
        void DeleteAnActivity(Guid ActivityID);
        IEnumerable<User> GetFriendList(Guid UserID);
    }
    public class UserService : IUserService
    {
        IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IEnumerable<User> GetUser()
        {
            return _userRepo.GetUser();
        }
        public User RetrieveUser(Guid UserID)
        {
            return _userRepo.GetUserByUserID(UserID);
        }
        //bool addNewUser(User user);
        public void AddNewUser(User User)
        {
           _userRepo.AddNewUser(User);
        }
        public bool AddFriendRelationship(Guid UserID_A, Guid UserID_B)
        {
            return _userRepo.AddFriendRelationship(UserID_A, UserID_B);
        }
        public bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B)
        {
            return _userRepo.DeleteFriendRelationship(UserID_A, UserID_B);
        }
        public bool UpdateUser(User User)
        {
            return _userRepo.UpdateUser(User);
        }
        public bool BanUser(Guid UserID)
        {
            return _userRepo.BanUser(UserID);
        }
        public bool ReportUser(Report Report)
        {
            return _userRepo.ReportUser(Report);
        }
        public IEnumerable<Report> GetReport()
        {
            return _userRepo.GetReport();
        }
        public bool UnbanUser(Guid UserID)
        {
            return _userRepo.UnbanUser(UserID);
        }
        public bool UnactiveUser(Guid UserID)
        {
            return _userRepo.UnactiveUser(UserID);
        }
        public bool GrantAdminPrivilege(Guid UserID)
        {
            return _userRepo.GrantAdminPrivilege(UserID);
        }
        public void DeleteAnActivity(Guid ActivityID)
        {
            _userRepo.DeleteAnActivity(ActivityID);
        }
        public IEnumerable<User> GetFriendList(Guid UserID)
        {
            return _userRepo.GetFriendList(UserID);
        }
    }
}