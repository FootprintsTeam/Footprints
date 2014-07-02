using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;

namespace Footprints.Areas.Friend.Controllers
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

        public PartialViewResult EachFriendArea(){
            return new PartialViewResult();
        }
	}
}