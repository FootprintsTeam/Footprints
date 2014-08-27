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
using System.IO;
using Footprints.Common;
using AutoMapper;
using Footprints.Helpers;
namespace Footprints.Controllers
{
    [Authorize]
    public class NewsfeedController : Controller
    {
        IUserService userService;
        INewsfeedService newsfeedService;
        IDestinationService destinationService;
        IJourneyService journeyService;
        ICommentService commentService;
        public NewsfeedController(IUserService userService, INewsfeedService newsfeedService, IDestinationService destinationService, IJourneyService journeyService, ICommentService commentService)
        {
            this.commentService = commentService;
            this.destinationService = destinationService;
            this.journeyService = journeyService;
            this.userService = userService;
            this.newsfeedService = newsfeedService;
        }

        //
        // GET: /Newsfeed/Newsfeed/
                
        
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
        public ActionResult Index()
        {
            var currentUser = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var newsfeedWidgets = newsfeedService.RetrieveNewsFeed(currentUser.UserID, Constant.defaultNewsfeedBlockNumber);

            var viewModels = NewConstructNewsfeedCollection(newsfeedWidgets);

            return View(viewModels);        
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

        //public ActionResult TemplateIndex() {
        //    return View();
        //}

        //[ChildActionOnly]
        //[ActionName("PersonalWidget1")]
        //public ActionResult PersonalWidget(object model)
        //{            
        //    return PartialView(model);
        //}

        [HttpPost]
        public ActionResult InfiniteScroll(int BlockNumber)
        {
            ////////////////// THis line of code only for demo. Needs to be removed ////
            //System.Threading.Thread.Sleep(1000);

            ////////////////////////////////////////////////////////////////////////////
            //int BlockSize = 5;
            //var books = DataManager.GetBooks(BlockNumber, BlockSize);

            IList<InfiniteScrollJsonModel> jsonModels = new List<InfiniteScrollJsonModel>();

            //jsonModel.NoMoreData = books.Count < BlockSize;

            var currentUser = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var activities = newsfeedService.LoadMoreNewsfeed(currentUser.UserID, Constant.defaultNewsfeedBlockNumber);
            var viewModels = NewConstructNewsfeedCollection(activities);
            foreach(var viewModel in viewModels){
                var viewName = viewModel.GetNewsfeedPartialViewName();
                jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = viewName == null ? "" : RenderPartialViewToString(viewName, viewModel) });
            }

            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("CommentWidget", CommentWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("DestinationWidget", DestinationWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("AddFriendWidget", AddFriendWidgetViewmodel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("JourneyWidget", JourneyWidgetViewModel.GetSampleObject()) });
            //jsonModels.Add(new InfiniteScrollJsonModel { HTMLString = RenderPartialViewToString("ShareWidget", ShareWidgetViewModel.GetSampleObject()) });

            return Json(jsonModels);
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult MainNavBar()
        {
            var userModel = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            return PartialView("MainNavBar", userModel);
        }

        [ChildActionOnly]
        public ActionResult AddFriendWidget()
        {
            var sample = AddFriendWidgetViewmodel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult CommentWidget()
        {
            var sample = CommentWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult ShareWidget()
        {
            var sample = ShareWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult PersonalWidget()
        {
            var model = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            PersonalWidgetViewModel viewModel = Mapper.Map<User, PersonalWidgetViewModel>(model);
            viewModel.NumberOfDestination = (int)userService.GetNumberOfDestination(viewModel.UserID);
            viewModel.NumberOfJourney = (int)userService.GetNumberOfJourney(viewModel.UserID);
            viewModel.NumberOfFriend = (int)userService.GetNumberOfFriend(viewModel.UserID);
            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult DestinationWidget()
        {
            var sample = DestinationWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult JourneyWidget()
        {
            var sample = JourneyWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }
    }
}