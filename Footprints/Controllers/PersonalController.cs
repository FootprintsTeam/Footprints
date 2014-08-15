﻿using System;
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
            var currentUserID = User.Identity.GetUserId();
            var model = userID.Equals("default") ? userService.RetrieveUser(new Guid(currentUserID)) : userService.RetrieveUser(new Guid(userID));
            
            var viewModel = Mapper.Map<User, PersonalViewModel>(model);

            if (!userID.Equals("default"))
                ViewBag.AlreadyFriend = userService.CheckFriendShip(new Guid(currentUserID), new Guid(userID));
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
                fileInforList = ImageProcessor.UploadPhoto(UserID.ToString(), UserID.ToString(), ContentID.ToString(), Request.Files.Get(0).InputStream, "");
                if (fileInforList != null && fileInforList.files.Count > 0)
                {
                    if (fileInforList.files.First().error == null)
                    {
                        var user = userService.RetrieveUser(UserID);
                        if (!user.CoverPhotoURL.Equals(Constant.DEFAULT_COVER_URL))
                        {
                            ImageProcessor.DeletePhoto(user.UserID.ToString(), user.UserID.ToString(), StringUtil.GetContentIdFromS3Url(user.CoverPhotoURL, user.UserID.ToString(), user.UserID.ToString()));
                        }
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
                fileInforList = ImageProcessor.UploadPhoto(UserID.ToString(), UserID.ToString(), ContentID.ToString(), Request.Files.Get(0).InputStream, "");
                if (fileInforList != null && fileInforList.files.Count > 0)
                {
                    if (fileInforList.files.First().error == null)
                    {
                        var user = userService.RetrieveUser(UserID);
                        if (!user.ProfilePicURL.Equals(Constant.DEFAULT_AVATAR_URL))
                        {
                            ImageProcessor.DeletePhoto(user.UserID.ToString(), user.UserID.ToString(), StringUtil.GetContentIdFromS3Url(user.ProfilePicURL, user.UserID.ToString(), user.UserID.ToString()));
                        }
                        userService.UpdateProfilePicURL(UserID, fileInforList.files.First().url);
                    }
                }
            }
            return Json(fileInforList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakeFriend(Guid userID)
        {
            var currentUserID = new Guid(User.Identity.GetUserId());
            bool result;

            //check if already is friend
            if (!userService.CheckFriendShip(userID, currentUserID))
            {
                 result = userService.AddFriendRelationship(currentUserID, userID);
            }
            else 
            {
                 result = userService.DeleteFriendRelationship(currentUserID, userID);
            }
                        
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}