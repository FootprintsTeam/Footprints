using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Services;

namespace Footprints.Areas.Journey.Controllers
{
    public class JourneyController : Controller
    {
        //
        // GET: /Journey/Journey/
        IJourneyService journeyService;

        public JourneyController(IJourneyService journeyService) {
            this.journeyService = journeyService;
        }
        public ActionResult Index()
        {
            var model = JourneyViewModel.GetSampleObject(Guid.NewGuid());
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewJourneyForm(AddNewJourneyViewModel journey) {
            var viewModel = journey;
            journeyService.AddJourney(viewModel);
            return View();
        }

        public string AddJourney() {
            return null;
        }
    }
}
