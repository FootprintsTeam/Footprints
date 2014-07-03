using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
namespace Footprints.Service
{
    public interface IJourneyService
    {
    }
    public class JourneyService : IJourneyService
    {
        IJourneyRepository _journeyRepo;
        public JourneyService(IJourneyRepository journeyRepo)
        {
            _journeyRepo = journeyRepo;
        }

        public int getNumberOfLikes(Guid journeyID)
        {
            return _journeyRepo.getNumberOfLikes(journeyID);
        }

        public Journey getJourneyByID(Guid journeyID)
        {
            return _journeyRepo.getJourneyByID(journeyID);
        }
    }
}