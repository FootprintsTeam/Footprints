using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;

namespace Footprints.Areas.Personal.Controllers
{
    public class PersonalController : Controller
    {
        //
        // GET: /Personal/Personal/
        public ActionResult Index()
        {
            var model = PersonalViewModel.GetSampleObject();
            return View(model);
        }
	}
}