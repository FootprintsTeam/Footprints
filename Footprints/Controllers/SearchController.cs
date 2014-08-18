using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Models;
using Footprints.Common;
using Footprints.Services;

namespace Footprints.Controllers
{
    public class SearchController : Controller
    {
        public ISearch SystemSearch;

        public SearchController(ISearch SystemSearch)
        {                        
            this.SystemSearch = SystemSearch;
        }

        //
        // GET: /Search/
        public ActionResult Journey(String TextEntered)
        {
            IList<Journey> list = SystemSearch.SearchJourney(TextEntered);
            return View(list);
        }

        public PartialViewResult JourneyPartial()
        {
            return new PartialViewResult();
        }
	}
}