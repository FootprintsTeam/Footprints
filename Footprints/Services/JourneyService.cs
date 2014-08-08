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
    }
}