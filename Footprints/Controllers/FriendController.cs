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
            System.Diagnostics.Debug.WriteLine("Friend Add");
            System.Diagnostics.Debug.WriteLine("UserID = [" + model.ToString() + "]");
            System.Diagnostics.Debug.WriteLine("FriendID = [" + model.ToString() + "]");
            userService.AddFriendRelationship(UserID, FriendID);
            var data = new List<AddFriendViewModel>();
            data.Add(model);
            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unfriend(AddFriendViewModel model)
        {
            System.Diagnostics.Debug.WriteLine("Friend Unfriend");
            System.Diagnostics.Debug.WriteLine("UserID = [" + model.ToString() + "]");
            System.Diagnostics.Debug.WriteLine("FriendID = [" + model.ToString() + "]");
            userService.DeleteFriendRelationship(model.UserID, model.FriendID);
            var data = new List<AddFriendViewModel>();
            data.Add(model);
            return Json(data, JsonRequestBehavior.DenyGet);
        }
    }
}
