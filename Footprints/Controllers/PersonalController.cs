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
using Footprints.ViewModels;

namespace Footprints.Controllers
{
    public class PersonalController : Controller
    {
        //
        // GET: /Personal/Personal/
        public ActionResult Index()
        {
            //var model = PersonalViewModel.GetSampleObject();
            //return View(model);
            return View();
        }
        public ActionResult About()
        {
            var model = PersonalAboutViewModel.GetSampleObject();
            return View(model);
        }
	}
}