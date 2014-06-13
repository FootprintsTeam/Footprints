using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;

namespace Footprints.Service
{
    public interface IJourneyService
    {

    }
    public class JourneyService : IJourneyService
    {
        IJourneyService _journeyRepo;
        public JourneyService(IJourneyService journeyRepo) {
            _journeyRepo = journeyRepo;
        }
    }
}