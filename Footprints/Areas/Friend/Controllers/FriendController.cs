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
            return null;
            
        }

        public PartialViewResult EachFriendArea(){
            return new PartialViewResult();
        }
	}
}