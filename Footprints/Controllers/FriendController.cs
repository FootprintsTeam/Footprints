using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Footprints.Controllers
{
    public class FriendController : Controller
    {
        //
        // GET: /Friend/Friend/
        public ActionResult Index()
        {
            var model = FriendViewModel.GetSampleObject();
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult FriendItem()
        {
            return new PartialViewResult();
        }
    }
}
