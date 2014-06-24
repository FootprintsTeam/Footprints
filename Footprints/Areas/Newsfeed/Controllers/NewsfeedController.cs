using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Areas.Newsfeed.Controllers
{
    public class NewsfeedController : Controller
    {
        //
        // GET: /Newsfeed/Newsfeed/
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult ItemJourneyPost()
        {
            return new PartialViewResult();
        }
	}
}