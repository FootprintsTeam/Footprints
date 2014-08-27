using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;
using Footprints.Services;
using AutoMapper;
using Footprints.Models;
using Microsoft.AspNet.Identity;
using Footprints.Common.JsonModel;
using System.IO;
namespace Footprints.Controllers
{
    public class JourneyController : Controller
    {
        IJourneyService journeyService;
        IDestinationService destinationService;
        IUserService userService;
        ICommentService commentService;
        public JourneyController(IJourneyService journeyService, IDestinationService destinationService, IUserService userService, ICommentService commentService)
        {
            this.journeyService = journeyService;
            this.destinationService = destinationService;
            this.userService = userService;
            this.commentService = commentService;
        }

        [ActionName("JourneySample")]
        public ActionResult Index()
        {
            var model = JourneyViewModel.GetSampleObject();
            model.JourneyID = new Guid("97d44e66-bd37-4f66-80d2-f6ac15b8923f");
            return View("Index", model);
        }
        //
        // GET: /Journey/
        public ActionResult Index(Guid journeyID)
        {
            //get current user
            var userID = new Guid(User.Identity.GetUserId());
            var journeyModel = journeyService.GetJourneyDetailWithComment(journeyID);
            var journeyViewModel = Mapper.Map<Journey, JourneyViewModel>(journeyModel);
            var journeyOwner = userService.RetrieveUser(journeyViewModel.UserID);
            journeyViewModel.NumberOfDestination = journeyViewModel.Destinations.Count();
            journeyViewModel.NumberOfLike = journeyService.GetNumberOfLike(journeyID);
            journeyViewModel.NumberOfShare = journeyService.GetNumberOfShare(journeyID);
            journeyViewModel.NumberOfPhoto = journeyService.GetNumberOfContent(journeyID);
            
            foreach (var x in journeyViewModel.Destinations)
            {
                Mapper.Map<User, DestinationViewModel>(journeyOwner, x);
            }
            if (journeyViewModel.Comments == null)
            {
                journeyViewModel.Comments = new List<Comment>();
            }
            journeyViewModel.AddNewDestinationFormViewModel = new AddNewDestinationFormViewModel { JourneyID = journeyID, TakenDate = DateTimeOffset.Now };
                        

            //check if user already like or share
            TempData["AlreadyLike"] = journeyService.UserAlreadyLiked(userID, journeyViewModel.JourneyID);
            TempData["IsAuthor"] = userID == journeyViewModel.UserID ? true : false;

            return View(journeyViewModel);
        }

        public ActionResult JourneyList(string userID = "default")
        {
            var targetUserID = new Guid();
            if (userID.Equals("default"))
            {
                targetUserID = new Guid(User.Identity.GetUserId());
            }
            else
            {
                targetUserID = new Guid(userID);
            }
            var journeyList = journeyService.GetJourneyDetailsListBelongToUser(targetUserID);
            if (journeyList == null)
            {
                journeyList = new List<Journey>();
            }
            return View(journeyList);
        }

        //
        // GET: /Journey/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Journey/Create
        [HttpPost]
        public ActionResult Create(AddNewJourneyViewModel model)
        {
            journeyService.AddJourney(model);
            return RedirectToAction("Index", "Journey", new { journeyID = model.JourneyID, username = User.Identity.GetUserName() });
        }

        //
        // POST: /Journey/Create

        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Journey/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(EditJourneyViewModel model)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var journey = Mapper.Map<EditJourneyViewModel, Journey>(model);
            journey.Timestamp = DateTimeOffset.Now;
            journey.UserID = userId;
            journeyService.UpdateJourney(userId, journey);
            return RedirectToAction("Index", "Journey", new { journeyID = model.JourneyID, username = User.Identity.GetUserName() });
        }

        //
        // GET: /Journey/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Journey/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid JourneyID)
        {
            var result = journeyService.DeleteJourney(new Guid(User.Identity.GetUserId()), JourneyID);
            return RedirectToAction("Index", "Newsfeed");
        }

        protected String RenderPartialViewToString(String viewName, object model)
        {
            if (String.IsNullOrEmpty(viewName))
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(CommentViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            var commentId = Guid.NewGuid();
            var userId = new Guid(User.Identity.GetUserId());
            comment.UserID = userId;
            comment.CommentID = commentId;
            comment.Timestamp = DateTimeOffset.Now;
            var commentObj = Mapper.Map<CommentViewModel, Comment>(comment);
            commentObj.Timestamp = DateTimeOffset.Now;
            var jsonModel = new CommentInfo();
            if (commentService.AddJourneyComment(userId, commentObj))
            {
                bool isPostedFromJourneyPage = Request.UrlReferrer.ToString().Contains("/Journey/Index");
                if (isPostedFromJourneyPage) TempData.Add("CommentPage", "Journey");
                var user = userService.RetrieveUser(userId);
                commentObj.User = user;
                jsonModel.HTMLString = RenderPartialViewToString("CommentItem", commentObj);
                if (isPostedFromJourneyPage) TempData.Remove("CommentPage");
            }
            else
            {
                jsonModel.HTMLString = "";
            }
            jsonModel.JourneyID = comment.JourneyID;
            return Json(jsonModel);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(CommentViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            var userId = new Guid(User.Identity.GetUserId());
            var commentObj = (Models.Comment)Mapper.Map<CommentViewModel, Models.Comment>(comment);
            //reset timestamp to current
            commentObj.Timestamp = DateTimeOffset.Now;
            //reset number of like
            IEnumerable<User> userLikedList = journeyService.GetAllUserLiked(comment.JourneyID);
            if (userLikedList != null)
            {
                commentObj.NumberOfLike = userLikedList.Count();
            }
            else
            {
                commentObj.NumberOfLike = 0;
            }
            var data = new List<CommentViewModel>();
            if (commentService.UpdateComment(userId, commentObj))
            {
                data.Add(comment);
            }
            return Json(data, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [Authorize]
        public Guid DeleteComment(Guid CommentID)
        {
            if (CommentID != null)
            {
                var userId = new Guid(User.Identity.GetUserId());
                commentService.DeleteAComment(userId, CommentID);
            }
            return CommentID;
        }
    }
}
