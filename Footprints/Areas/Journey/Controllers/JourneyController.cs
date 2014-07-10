using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Areas.Journey.Controllers
{
    public class JourneyController : Controller
    {
        //
        // GET: /Journey/Journey/
        public ActionResult Index()
        {
            var model = JourneyViewModel.GetSampleObject();
            return View(model);
        }

        public string AddJourney() {

            return null;
        }
    }
}
