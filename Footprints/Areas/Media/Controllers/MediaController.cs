﻿using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Areas.Media.Controllers
{
    public class MediaController : Controller
    {
        //
        // GET: /Media/Media/
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ItemMediaAlbum()
        {
            return new PartialViewResult();
        }
    }
}
