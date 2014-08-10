using AutoMapper;
using Footprints.Services;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Footprints.Models;
using Footprints.Common;
using System.IO;
namespace Footprints.Controllers
{
    public class DestinationController : Controller
    {
        IDestinationService destinationService;
        ICommentService commentService;
        IUserService userService;
        IJourneyService journeyService;
        public DestinationController(IDestinationService destinationService, ICommentService commentService, IUserService userService, IJourneyService journeyService)
        {
            this.destinationService = destinationService;
            this.commentService = commentService;
            this.userService = userService;
            this.journeyService = journeyService;
        }

        //
        // GET: /Destination/
        [ActionName("DestinationSample")]
        public ActionResult Index()
        {
            TempData["AlreadyLike"] = destinationService.UserAlreadyLike(new Guid(User.Identity.GetUserId()), Guid.NewGuid());
            var model = Footprints.ViewModels.DestinationViewModel.GetSampleObject();
            return View("Index", model);
        }

        public ActionResult Index(Guid destinationID)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var destinationModel = destinationService.GetDestination(destinationID);
            var destinationViewModel = Mapper.Map<Destination, DestinationViewModel>(destinationModel);
            destinationViewModel.Place = destinationService.GetDestinationPlace(destinationID);
            var comments = commentService.RetrieveDestinationComment(destinationID);
            if (comments.Count > 0)
            {
                foreach (Comment comment in comments)
                {
                    destinationViewModel.Comments.Add(Mapper.Map<Comment, CommentViewModel>(comment));
                }
            }
            destinationViewModel.NumberOfJourney = journeyService.GetJourneyListBelongToUser(userId).Count;
            destinationViewModel.NumberOfDestination = destinationService.GetAllDestination().Count();
            destinationViewModel.NumberOfFriend = 0;
            destinationViewModel.NumberOfLike = destinationService.GetAllUserLiked(destinationID).Count();
            destinationViewModel.NumberOfShare = destinationService.GetAllUserShared(destinationID).Count();
            //destinationViewModel.Place
            destinationViewModel.EditDestinationForm = Mapper.Map<DestinationViewModel, EditDestinationFormViewModel>(destinationViewModel);
            Mapper.Map<User, DestinationViewModel>(userService.RetrieveUser(destinationViewModel.UserID),destinationViewModel);       

            //check if user already like or share
            TempData["AlreadyLike"] = destinationService.UserAlreadyLike(new Guid(User.Identity.GetUserId()), destinationID);
            return View(destinationViewModel);
        }

        //
        // GET: /Destination/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // POST: /Destination/Create
        [HttpPost]
        public ActionResult Create(AddNewDestinationFormViewModel model)
        {
            //Get UserId
            model.DestinationID = Guid.NewGuid();            
            var place = Mapper.Map<AddNewDestinationFormViewModel, Place>(model);
            var destination = Mapper.Map<AddNewDestinationFormViewModel, Destination>(model);
            destination.UserID = new Guid(User.Identity.GetUserId());
            destinationService.AddNewDestination(destination.UserID, destination, place, model.JourneyID);
            return RedirectToAction("Index", "Destination", new { destinationID = destination.DestinationID });
        }


        //
        // POST: /Destination/Edit/5
        [HttpPost]
        public ActionResult Edit(AddNewDestinationFormViewModel model)
        {
            //destinationService.UpdateDestination(Mapper.Map<AddNewDestinationFormViewModel, Models.Destination>(model));
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, Guid JourneyID)
        {
            var user = Membership.GetUser(User.Identity.Name);
            Guid userId = (Guid)user.ProviderUserKey;
            destinationService.DeleteDestination(userId, id);
            //Redirect to Journey
            return RedirectToAction("Index", "Journey", new { id = JourneyID });
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
            var commentId = Guid.NewGuid();
            var userId = new Guid(User.Identity.GetUserId());
            comment.CommentID = commentId;
            comment.UserCommentName = User.Identity.GetUserName();
            comment.Time = DateTimeOffset.Now;
            comment.NumberOfLike = 0;
            var commentObj = (Models.Comment)Mapper.Map<CommentViewModel, Models.Comment>(comment);
            //reset timestamp to current
            commentObj.Timestamp = DateTimeOffset.Now;
            commentObj.NumberOfLike = 0;
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();
            if (commentService.AddDestinationComment(userId, commentObj))
            {
                jsonModel.HTMLString = RenderPartialViewToString("CommentItem", comment);
            }
            else
            {
                jsonModel.HTMLString = "";
            }
            return Json(jsonModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(CommentViewModel comment)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var commentObj = (Models.Comment)Mapper.Map<CommentViewModel, Models.Comment>(comment);
            //reset timestamp to current
            commentObj.Timestamp = DateTimeOffset.Now;
            //reset number of like
            IEnumerable<User> userLikedList = destinationService.GetAllUserLiked(comment.DestinationID);
            commentObj.NumberOfLike = userLikedList.Count();
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
                
            }
            //RedirectToAction("Index", "Home");
            return CommentID;
        }

        public ActionResult AddNewPhoto() {
            var photoContent = TempData["FileInfoList"];
            var destinationId = TempData["MasterID"];
            //add Content here

            //delete temporary data
            TempData.Remove("FileInfoList");
            TempData.Remove("MasterID");
            return Json(photoContent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LikeUnlike(Guid userID, Guid destinationID)
        {
            string result;

            try
            {
                destinationService.LikeDestination(userID, destinationID);
                 result = FunctionResult.success.ToString();
            } catch(Exception e){
                result = e.Message;
            }

            return Json(new { Result = result});
        }

        [ChildActionOnly]
        public ActionResult EditCommentForm()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult CommentSection(List<CommentViewModel> viewModel)
        {
            return PartialView("CommentSection", viewModel);
        }

        [ChildActionOnly]
        public ActionResult EditDestinationForm(EditDestinationFormViewModel viewModel)
        {
            return PartialView("EditDestinationForm", viewModel);
        }

        [ChildActionOnly]
        public ActionResult DestinationMainContentWidget(DestinationViewModel viewModel) {            
            return PartialView("DestinationMainContentWidget", viewModel);
        }
    }
}
