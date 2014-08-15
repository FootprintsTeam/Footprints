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
using Footprints.Services;
using AutoMapper;
using Footprints.Common.JsonModel;
using Footprints.Common;

namespace Footprints.Controllers
{
    public class PersonalController : Controller
    {
        IUserService userService;
        IJourneyService journeyService;
        IDestinationService destinationService;

        public PersonalController(IUserService userService, IJourneyService journeyService, IDestinationService destinationService)
        {
            this.userService = userService;
            this.journeyService = journeyService;
            this.destinationService = destinationService;
        }
        //
        // GET: /Personal/Personal/
        public ActionResult Index(string userID = "default")
        {            
            var model = userID.Equals("default") ? userService.RetrieveUser(new Guid(User.Identity.GetUserId())) : userService.RetrieveUser(new Guid(userID));
            
            var viewModel = Mapper.Map<User, PersonalViewModel>(model);

            //var model = PersonalViewModel.GetSampleObject();            
            return View(viewModel);
        }
        public ActionResult About()
        {
            var model = PersonalAboutViewModel.GetSampleObject();
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(PersonalAboutViewModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Update()
        {
            var model = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var viewModel = Mapper.Map<User, PersonalAboutViewModel>(model);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddCoverPhoto()
        {
            FileInfoList fileInforList = null;
            if (Request.Files.Count > 0)
            {
                var UserID = new Guid(User.Identity.GetUserId());
                var ContentID = Guid.NewGuid();
                string deleteUrl = Url.Action("DeletePhoto", "Media", new { id = ContentID });
                fileInforList = ImageProcessor.UploadPhoto(UserID, UserID, ContentID, Request.Files.Get(0).InputStream, deleteUrl);
                if (fileInforList != null && fileInforList.files.Count > 0)
                {
                    if (fileInforList.files.First().error == null)
                    {
                        userService.UpdateCoverPhotoURL(UserID, fileInforList.files.First().url);
                    }
                }
            }
            return Json(fileInforList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddAvatarPhoto()
        {
            FileInfoList fileInforList = null;
            if (Request.Files.Count > 0)
            {
                var UserID = new Guid(User.Identity.GetUserId());
                var ContentID = Guid.NewGuid();
                string deleteUrl = Url.Action("DeletePhoto", "Media", new { id = ContentID });
                fileInforList = ImageProcessor.UploadPhoto(UserID, UserID, ContentID, Request.Files.Get(0).InputStream, deleteUrl);
                if (fileInforList != null && fileInforList.files.Count > 0)
                {
                    if (fileInforList.files.First().error == null)
                    {
                        userService.UpdateProfilePicURL(UserID, fileInforList.files.First().url);
                    }
                }
            }
            return Json(fileInforList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakeFriend(Guid userID)
        {
            var currentUserID = new Guid(User.Identity.GetUserId());
            var result = userService.AddFriendRelationship(currentUserID, userID);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}