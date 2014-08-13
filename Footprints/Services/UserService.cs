using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;

namespace Footprints.Services
{
    public interface IUserService {
        IList<User> GetUser();
        User RetrieveUser(Guid UserID);
        //bool addNewUser(User user);
        void AddNewUser(User User);
        bool AddFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool DeleteFriendRelationship(Guid UserID_A, Guid UserID_B);
        bool UpdateUser(User User);
        bool BanUser(Guid UserID);
        bool ReportUser(Report Report);
        IList<Report> GetReport();
        bool UnbanUser(Guid UserID);
        bool UnactiveUser(Guid UserID);
        bool GrantAdminPrivilege(Guid UserID);
        void DeleteAnActivity(Guid ActivityID);
        IList<User> GetFriendList(Guid UserID);
        long GetNumberOfJourney(Guid UserID);
        long GetNumberOfDestination(Guid UserID);
        long GetNumberOfFriend(Guid UserID);
        bool DeleteUser(Guid UserID);
        bool UpdateProfilePicURL(Guid UserID, String ProfilePicURL);
        bool UpdateCoverPhotoURL(Guid UserID, String CoverPhotoURL);
        bool ChangePassword(Guid UserID, String Password);
    }
    public class UserService : IUserService
    {
        IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IList<User> GetUser()
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
        public IList<Report> GetReport()
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
        public IList<User> GetFriendList(Guid UserID)
        {
            return _userRepo.GetFriendList(UserID);
        }
        public long GetNumberOfJourney(Guid UserID)
        {
            return _userRepo.GetNumberOfJourney(UserID);
        }
        public long GetNumberOfDestination(Guid UserID)
        {
            return _userRepo.GetNumberOfDestination(UserID);
        }
        public long GetNumberOfFriend(Guid UserID)
        {
            return _userRepo.GetNumberOfFriend(UserID);
        }
        public bool DeleteUser(Guid UserID)
        {
            return _userRepo.DeleteUser(UserID);
        }
        public bool UpdateProfilePicURL(Guid UserID, String ProfilePicURL)
        {
            return _userRepo.UpdateProfilePicURL(UserID, ProfilePicURL);
        }
        public bool UpdateCoverPhotoURL(Guid UserID, String CoverPhotoURL)
        {
            return _userRepo.UpdateCoverPhotoURL(UserID, CoverPhotoURL);
        }
        public bool ChangePassword(Guid UserID, String Password)
        {
            return _userRepo.ChangePassword(UserID, Password);
        }
    }
}