using Footprints.Services;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Footprints.Models;
using AutoMapper;
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
        public ActionResult Index(string userID = "default")
        {
            IList<User> friendList;
            if (userID != "default")
            {
                friendList = userService.GetFriendList(new Guid(userID));                
            }
            else
            {
                friendList = userService.GetFriendList(new Guid(User.Identity.GetUserId()));
            }

            var viewModel = new FriendViewModel();

            foreach (User user in friendList) {
                viewModel.FriendList.Add(Mapper.Map<User,FriendItemViewModel>(user));
            };
            
            return View(viewModel);
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
