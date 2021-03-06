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
using System.IO;
using Footprints.Helpers;
namespace Footprints.Controllers
{
    public class PersonalController : Controller
    {
        IUserService userService;
        IJourneyService journeyService;
        IDestinationService destinationService;
        ICommentService commentService;
        INewsfeedService newsfeedService;
        private static int skip;
        public PersonalController(IUserService userService, IJourneyService journeyService, IDestinationService destinationService, ICommentService commentService, INewsfeedService newsfeedService)
        {
            this.userService = userService;
            this.journeyService = journeyService;
            this.destinationService = destinationService;
            this.commentService = commentService;
            this.newsfeedService = newsfeedService;
        }

        //
        // GET: /Personal/Personal/

        public IList<NewsfeedBaseWidgetViewModel> NewConstructNewsfeedCollection(IList<Activity> newsfeedWidgets)
        {

            IList<NewsfeedBaseWidgetViewModel> viewModels = new List<NewsfeedBaseWidgetViewModel>();

            if (newsfeedWidgets == null)
            {
                return viewModels;
            }

            foreach (var activity in newsfeedWidgets)
            {

                switch (activity.Type)
                {
                    case Constant.ActivityAddNewContent:
                        var destinationPhoto = destinationService.GetDestinationDetail(activity.DestinationID);
                        AddPhotoWidgetViewModel photoModel = Mapper.Map<Activity, AddPhotoWidgetViewModel>(activity);
                        viewModels.Add(photoModel);
                        break;

                    case Constant.ActivityAddNewDestination:
                        DestinationWidgetViewModel destinationModel = Mapper.Map<Activity, DestinationWidgetViewModel>(activity);
                        //Mapper.Map<Destination,DestinationWidgetViewModel>()                        
                        destinationModel.Comments = Mapper.Map<IList<Comment>, IList<CommentViewModel>>(commentService.RetrieveDestinationComment(activity.DestinationID));
                        destinationModel.Place = Mapper.Map<Activity, Place>(activity);
                        viewModels.Add(destinationModel);
                        break;

                    case Constant.ActivityAddNewFriend:
                        AddFriendWidgetViewmodel addFriendModel = Mapper.Map<Activity, AddFriendWidgetViewmodel>(activity);
                        viewModels.Add(addFriendModel);
                        break;

                    case Constant.ActivityAddnewJourney:
                        JourneyWidgetViewModel journeyModel = Mapper.Map<Activity, JourneyWidgetViewModel>(activity);
                        Mapper.Map<Journey, JourneyWidgetViewModel>(journeyService.RetrieveJourney(activity.JourneyID), journeyModel);
                        viewModels.Add(journeyModel);
                        break;

                    case Constant.ActivityComment:
                        CommentWidgetViewModel commentModel = Mapper.Map<Activity, CommentWidgetViewModel>(activity);
                        var commentList = commentService.RetrieveDestinationComment(activity.DestinationID);
                        commentModel.Comments = Mapper.Map<IList<Comment>, IList<CommentViewModel>>(commentList);
                        commentModel.Place = Mapper.Map<Activity, Place>(activity);
                        viewModels.Add(commentModel);
                        break;

                    case Constant.ActivityLikeDestination:
                        DestinationWidgetViewModel likeDestinationModel = Mapper.Map<Activity, DestinationWidgetViewModel>(activity);
                        var commenLiketList = commentService.RetrieveDestinationComment(activity.DestinationID);
                        likeDestinationModel.Comments = Mapper.Map<IList<Comment>, IList<CommentViewModel>>(commenLiketList);
                        likeDestinationModel.Place = Mapper.Map<Activity, Place>(activity);
                        viewModels.Add(likeDestinationModel);
                        break;

                    case Constant.ActivityShareDestination:
                        ShareWidgetViewModel shareModel = Mapper.Map<Activity, ShareWidgetViewModel>(activity);
                        var commentShareList = commentService.RetrieveDestinationComment(activity.DestinationID);
                        shareModel.Comments = Mapper.Map<IList<Comment>, IList<CommentViewModel>>(commentShareList);
                        shareModel.Place = Mapper.Map<Activity, Place>(activity);
                        viewModels.Add(shareModel);
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine(activity.Type + "something");
                        break;
                }
            };

            return viewModels.OrderByDescending(x => x.Timestamp).ToList<NewsfeedBaseWidgetViewModel>();
        }

        [HttpPost]
        public ActionResult InfiniteScroll(int BlockNumber, Guid UserID)
        {
            ////////////////// THis line of code only for demo. Needs to be removed ////
            IList<InfiniteScrollJsonModel> jsonModels = new List<InfiniteScrollJsonModel>();

            //jsonModel.NoMoreData = books.Count < BlockSize;            
            var currentUser = userService.RetrieveUser(UserID);
            var activities = userService.GetAllActivity(currentUser.UserID, skip, Constant.defaultNewsfeedBlockNumber);
            skip = skip + Constant.defaultNewsfeedBlockNumber;
            var viewModels = NewConstructNewsfeedCollection(activities);
            foreach (var viewModel in viewModels)
            {
                var viewName = viewModel.GetPersonalPartialViewName();
                jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = viewName.Equals("false") ? "" : RenderPartialViewToString(viewName, viewModel) });
            }

            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("CommentWidget", CommentWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("DestinationWidget", DestinationWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("AddFriendWidget", AddFriendWidgetViewmodel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("JourneyWidget", JourneyWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("ShareWidget", ShareWidgetViewModel.GetSampleObject()) });

            return Json(jsonModels);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext
                (ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult Index(string userID = "default")
        {
            skip = 0;
            var currentUserID = User.Identity.GetUserId();
            var model = userID.Equals("default") ? userService.RetrieveUser(new Guid(currentUserID)) : userService.RetrieveUser(new Guid(userID));

            var viewModel = Mapper.Map<User, PersonalViewModel>(model);
            viewModel.Activities = NewConstructNewsfeedCollection(userService.GetAllActivity(viewModel.UserID, skip, Constant.defaultNewsfeedBlockNumber));
            skip = skip + Constant.defaultNewsfeedBlockNumber;
            //add number of pictures
            viewModel.NumberOfPhoto = userService.GetNumberOfContentByUserID(viewModel.UserID);
            viewModel.NumberOfJourney = (int)userService.GetNumberOfJourney(viewModel.UserID);
            viewModel.NumberOfDestination = destinationService.GetNumberOfDestination(viewModel.UserID);
            viewModel.NumberOfFriend = (int)userService.GetNumberOfFriend(viewModel.UserID);
            if (!userID.Equals("default"))
                ViewBag.AlreadyFriend = userService.CheckFriendShip(new Guid(currentUserID), new Guid(userID));
            //var model = PersonalViewModel.GetSampleObject();      

            return View(viewModel);

        }
        public ActionResult About(string userID = "default")
        {
            var currentUserID = User.Identity.GetUserId();
            var model = userID.Equals("default") ? userService.RetrieveUser(new Guid(currentUserID)) : userService.RetrieveUser(new Guid(userID));
            var viewModel = Mapper.Map<User, PersonalViewModel>(model);
            //add number of pictures
            viewModel.NumberOfPhoto = userService.GetNumberOfContentByUserID(viewModel.UserID);
            viewModel.NumberOfJourney = (int)userService.GetNumberOfJourney(viewModel.UserID);
            viewModel.NumberOfDestination = destinationService.GetNumberOfDestination(viewModel.UserID);
            viewModel.NumberOfFriend = (int)userService.GetNumberOfFriend(viewModel.UserID);
            if (!userID.Equals("default"))
                ViewBag.AlreadyFriend = userService.CheckFriendShip(new Guid(currentUserID), new Guid(userID));
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(PersonalViewModel model)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Update()
        {
            var model = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var viewModel = Mapper.Map<User, PersonalViewModel>(model);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto(FormCollection form)
        {
            var subActionName = form.GetValue("SubActionName");
            if (subActionName != null && subActionName.AttemptedValue.Equals("AddCoverPhoto"))
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
            else
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