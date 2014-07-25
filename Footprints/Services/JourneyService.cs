using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Footprints.ViewModels;
using AutoMapper;
namespace Footprints.Service
{
    public interface IJourneyService
    {
        void AddJourney(AddNewJourneyViewModel journeyViewModel);        
        Journey RetrieveJourney(Guid JourneyID);
        Journey GetJourneyDetail(Guid JourneyID);
        bool AddNewJourney(Guid UserID, Journey Journey);
        bool UpdateJourney(Journey Journey);
        bool DeleteJourney(Guid JourneyID);
        //For Admin Page
        IEnumerable<Journey> GetJourneyList();
        //For Personal Page
        IEnumerable<Journey> GetJourneyListBelongToUser(Guid UserID);
        void LikeJourney(Guid UserID, Guid JourneyID);
        void UnlikeJourney(Guid UserID, Guid JourneyID);
        IEnumerable<User> GetAllUserLiked(Guid JourneyID);
        void ShareJourney(Guid UserID, Guid JourneyID, String Content);
        IEnumerable<User> GetAllUserShared(Guid JourneyID);
        int GetNumberOfShare(Guid JourneyID);
        int GetNumberOfLike(Guid JourneyID);
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
            journeyRepository.AddNewJourney(new Guid(),model);
        }

        public int GetNumberOfLike(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfLike(JourneyID);
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

        public bool UpdateJourney(Journey Journey)
        {
            return journeyRepository.UpdateJourney(Journey);
        }

        public bool DeleteJourney(Guid JourneyID)
        {
            return journeyRepository.DeleteJourney(JourneyID);
        }

        public IEnumerable<Journey> GetJourneyList()
        {
            return journeyRepository.GetJourneyList();
        }
        IEnumerable<Journey> GetJourneyListBelongToUser(Guid UserID)
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

        public IEnumerable<User> GetAllUserLiked(Guid JourneyID)
        {
            return journeyRepository.GetAllUserLiked(JourneyID);
        }
        public void ShareJourney(Guid UserID, Guid JourneyID, String Content)
        {
            journeyRepository.ShareJourney(UserID, JourneyID, Content);
        }

        public IEnumerable<User> GetAllUserShared(Guid JourneyID)
        {
            return journeyRepository.GetAllUserShared(JourneyID);
        }

        public int GetNumberOfShare(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfShare(JourneyID);
        }

        public int GetNumberOfLike(Guid JourneyID)
        {
            return journeyRepository.GetNumberOfLike(JourneyID);
        }
    }
}