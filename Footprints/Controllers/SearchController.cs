using Footprints.Common;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Controllers
{
    public class SearchController : Controller
    {
        ISearch search;
        public SearchController(ISearch search)
        {
            this.search = search;
        }
        public ActionResult Index(string keyword)
        {
            SearchViewModel model = new SearchViewModel();
            model.Destinations = search.SearchDestination(keyword, 10);
            model.Journeys = search.SearchJourney(keyword, 10);
            model.Places = search.SearchPlace(keyword, 10);
            model.Users = search.SearchUser(keyword, 10);
            return View();
        }
	}
}