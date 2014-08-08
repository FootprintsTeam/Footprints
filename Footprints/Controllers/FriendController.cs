using Footprints.Services;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Footprints.Controllers
{
    public class FriendController : Controller
    {
        IUserService userService;

        public FriendController(IUserService userService)
        {
            this.userService = userService;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddFriendViewModel model)
        {
            userService.AddFriendRelationship(model.UserID, model.FriendID);
            var data = new List<AddFriendViewModel>();
            data.Add(model);
            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unfriend(AddFriendViewModel model)
        {
            userService.DeleteFriendRelationship(model.UserID, model.FriendID);
            var data = new List<AddFriendViewModel>();
            data.Add(model);
            return Json(data, JsonRequestBehavior.DenyGet);
        }
    }
}
