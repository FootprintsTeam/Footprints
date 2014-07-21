using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Footprints.Models;

namespace Footprints.Controllers
{
    public class NewsfeedController : Controller
    {
        //
        // GET: /Newsfeed/Newsfeed/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TemplateIndex() {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ItemJourneyPost()
        {
            return new PartialViewResult();
        }
	}
}