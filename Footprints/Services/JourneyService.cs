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

        public int getNumberOfLikes(Guid journeyID)
        {
            return journeyRepository.GetNumberOfLike(journeyID);
        }

        public Journey getJourneyByID(Guid journeyID)
        {
            return journeyRepository.GetJourneyByID(journeyID);
        }
    }
}