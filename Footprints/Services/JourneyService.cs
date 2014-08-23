using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Footprints.ViewModels;
using AutoMapper;
namespace Footprints.Services
{
    public interface IJourneyService
    {
        void AddJourney(AddNewJourneyViewModel journeyViewModel);        
        Journey RetrieveJourney(Guid JourneyID);
        Journey GetJourneyDetail(Guid JourneyID);
        bool AddNewJourney(Guid UserID, Journey Journey);
        bool UpdateJourney(Guid UserID, Journey Journey);
        bool DeleteJourney(Guid UserID, Guid JourneyID);
        //For Admin Page
        IList<Journey> GetJourneyList();
        //For Personal Page
        IList<Journey> GetJourneyListBelongToUser(Guid UserID);
        void LikeJourney(Guid UserID, Guid JourneyID);
        void UnlikeJourney(Guid UserID, Guid JourneyID);
        IList<User> GetAllUserLiked(Guid JourneyID);
        void ShareJourney(Guid UserID, Guid JourneyID, String Content);
        IList<User> GetAllUserShared(Guid JourneyID);
        IList<Journey> GetAllJourney();
        int GetNumberOfJourney();
        int GetNumberOfLike(Guid JourneyID);
        int GetNumberOfShare(Guid JourneyID);
        bool UserAlreadyLiked(Guid UserID, Guid JourneyID);
        bool UserAlreadyShared(Guid UserID, Guid JourneyID);
        bool UpdateJourneyForAdmin(Journey Journey);
        bool UpdateJourney(Guid UserID, Guid JourneyID, String Name, String Description, DateTimeOffset TakenDate, DateTimeOffset Timestamp);
        int GetNumberOfContent(Guid JourneyID);
        IList<Journey> GetJourneyDetailsListBelongToUser(Guid UserID);
        Journey GetJourneyDetailWithComment(Guid JourneyID);
    }
    public class JourneyService : IJourneyService
    {
        IJourneyRepository journeyRepository;
        public JourneyService(IJourneyRepository journeyRepo)
        {
            this.journeyRepository = journeyRepo;
        }
        public void AddJourney(AddNewJourneyViewModel journeyViewModel) {
            var model =  Mapper.Map<AddNewJourneyViewModel, Journey>(journeyViewModel);
            journeyRepository.AddNewJourney(model.UserID, model);
        }
        public Journey RetrieveJourney(Guid JourneyID)
        {
            return journeyRepository.GetJourneyByID(JourneyID);
        }
        public Journey GetJourneyDetail(Guid JourneyID)
        {
            return journeyRepository.GetJourneyDetail(JourneyID);
        }
        public bool AddNewJourney(Guid UserID, Journey Journey)
        {
            return journeyRepository.AddNewJourney(UserID, Journey);
        }
        public bool UpdateJourney(Guid UserID, Journey Journey)
        {
            return journeyRepository.UpdateJourney(UserID, Journey);
        }
        public bool DeleteJourney(Guid UserID, Guid JourneyID)
        {
            return journeyRepository.DeleteJourney(UserID, JourneyID);
        }
        public IList<Journey> GetJourneyList()
        {
            return journeyRepository.GetJourneyList();
        }
        public IList<Journey> GetJourneyListBelongToUser(Guid UserID)
        {
            return journeyRepository.GetJourneyListBelongToUser(UserID);
        }
        public void LikeJourney(Guid UserID, Guid JourneyID)
        {
            journeyRepository.LikeJourney(UserID, JourneyID);
        }
        public void UnlikeJourney(Guid UserID, Guid JourneyID)
        {
            journeyRepository.UnlikeJourney(UserID, JourneyID);
        }
        public IList<User> GetAllUserLiked(Guid JourneyID)
        {
            return journeyRepository.GetAllUserLiked(JourneyID);
        }
        public void ShareJourney(Guid UserID, Guid JourneyID, String Content)
        {
            journeyRepository.ShareJourney(UserID, JourneyID, Content);
        }
        public IList<User> GetAllUserShared(Guid JourneyID)
        {
            return journeyRepository.GetAllUserShared(JourneyID);
        }
        public IList<Journey> GetAllJourney()
        {
            return journeyRepository.GetAllJourney();
        }
        public int GetNumberOfJourney()
        {
            return journeyRepository.GetNumberOfJourney();
        }
        public int GetNumberOfLike(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfLike(JourneyID);
        }
        public int GetNumberOfShare(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfShare(JourneyID);
        }
        public bool UserAlreadyLiked(Guid UserID, Guid JourneyID)
        {
            return journeyRepository.UserAlreadyLiked(UserID, JourneyID);
        }
        public bool UserAlreadyShared(Guid UserID, Guid JourneyID)
        {
            return journeyRepository.UserAlreadyShared(UserID, JourneyID);
        }
        public bool UpdateJourneyForAdmin(Journey Journey)
        {
            return journeyRepository.UpdateJourneyForAdmin(Journey);
        }
        public bool UpdateJourney(Guid UserID, Guid JourneyID, String Name, String Description, DateTimeOffset TakenDate, DateTimeOffset Timestamp)
        {
            return journeyRepository.UpdateJourney(UserID, JourneyID, Name, Description, TakenDate, Timestamp);
        }
        public int GetNumberOfContent(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfContent(JourneyID);
        }
        public IList<Journey> GetJourneyDetailsListBelongToUser(Guid UserID)
        {
            return journeyRepository.GetJourneyDetailsListBelongToUser(UserID);
        }
        public Journey GetJourneyDetailWithComment(Guid JourneyID)
        {
            return journeyRepository.GetJourneyDetailWithComment(JourneyID);
        }
    }
}