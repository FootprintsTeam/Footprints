﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;
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