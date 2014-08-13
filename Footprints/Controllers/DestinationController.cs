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
            //current user
            var userId = new Guid(User.Identity.GetUserId());
            //destination model
            var destinationModel = destinationService.GetDestination(destinationID);
            var destinationViewModel = Mapper.Map<Destination, DestinationViewModel>(destinationModel);
            destinationViewModel.Place = destinationService.GetDestinationPlace(destinationID);
            var comments = commentService.RetrieveDestinationComment(destinationID);
            if (comments.Count > 0)
            {
                destinationViewModel.Comments = new List<CommentViewModel>();
                foreach (var comment in comments)
                {
                    destinationViewModel.Comments.Add(Mapper.Map<Comment, CommentViewModel>(comment));
                }
            }
            destinationViewModel.NumberOfJourney = journeyService.GetJourneyListBelongToUser(destinationModel.UserID).Count;
            destinationViewModel.NumberOfDestination = destinationService.GetNumberOfDestination(destinationModel.UserID);
            destinationViewModel.NumberOfFriend = (int)userService.GetNumberOfFriend(destinationModel.UserID);
            destinationViewModel.EditDestinationForm = Mapper.Map<DestinationViewModel, EditDestinationFormViewModel>(destinationViewModel);
            Mapper.Map<User, DestinationViewModel>(userService.RetrieveUser(destinationViewModel.UserID), destinationViewModel);

            //check if user already like or share
            TempData["AlreadyLike"] = destinationService.UserAlreadyLike(userId, destinationID);
            TempData["IsDestinationAuthor"] = userId == destinationModel.UserID ? true : false;
            return View(destinationViewModel);
        }

        /// <summary>
        /// Create a new Destination
        /// </summary>
        /// <param name="model">Contains Destination information</param>
        /// <returns>success: redirect to the new Destination; fail: redirect to the referrer</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddNewDestinationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            var place = Mapper.Map<AddNewDestinationFormViewModel, Place>(model);
            place.Name = model.PlaceName;
            var destination = Mapper.Map<AddNewDestinationFormViewModel, Destination>(model);
            destination.UserID = new Guid(User.Identity.GetUserId());
            destination.AlbumID = Guid.NewGuid();
            destination.DestinationID = Guid.NewGuid();
            destination.Timestamp = DateTimeOffset.Now;
            try
            {
                if (destinationService.AddNewDestination(destination.UserID, destination, place, model.JourneyID))
                {
                    return RedirectToAction("Index", "Destination", new { destinationID = destination.DestinationID });
                }
                else
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            catch
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            
        }

        /// <summary>
        /// Edit information of Destination
        /// </summary>
        /// <param name="model">Contains Destination information</param>
        /// <returns>success: reload the current destination page; fail: redirect to the referrer</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditDestinationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            var userId = new Guid(User.Identity.GetUserId());
            var place = Mapper.Map<EditDestinationFormViewModel, Place>(model);
            place.Name = model.PlaceName;
            var destination = Mapper.Map<EditDestinationFormViewModel, Destination>(model);
            destination.AlbumID = Guid.NewGuid();
            destination.Timestamp = DateTimeOffset.Now;
            destination.UserID = userId;
            destination.Place = place;
            try
            {
                if (destinationService.UpdateDestination(userId, destination))
                {
                    return RedirectToAction("Index", "Destination", new { destinationID = destination.DestinationID });
                }
                else
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            catch
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        /// <summary>
        /// Delete a Destination
        /// </summary>
        /// <param name="model">Contains Destination information</param>
        /// <returns>success: redirect to the Journey which the current Destination belongs to; fail: return to the referrer page</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteDestinationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            var userId = new Guid(User.Identity.GetUserId());
            destinationService.DeleteDestination(userId, model.DestinationID);
            return RedirectToAction("Index", "Journey", new { id = model.JourneyID });
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

        /// <summary>
        /// Add a Comment to the current Destination
        /// </summary>
        /// <param name="comment">Contains information of comment</param>
        /// <returns>success: returns json which contains information of comment; fail: return empty json</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(CommentViewModel comment)
        {
            var commentId = Guid.NewGuid();
            var userId = new Guid(User.Identity.GetUserId());
            var user = userService.RetrieveUser(userId);
            comment.UserAvatarURL = user.ProfilePicURL;
            comment.UserCommentId = user.UserID;
            comment.CommentID = commentId;
            comment.UserCommentName = User.Identity.GetUserName();
            comment.Time = DateTimeOffset.Now;
            comment.NumberOfLike = 0;
            var commentObj = Mapper.Map<CommentViewModel, Comment>(comment);
            commentObj.Timestamp = DateTimeOffset.Now;
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

        /// <summary>
        /// Edit a comment of the current Destination
        /// </summary>
        /// <param name="comment">Contains information of the comment</param>
        /// <returns>success: returns json which contains information of comment; fail: return empty json</returns>
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
                var userId = new Guid(User.Identity.GetUserId());
                commentService.DeleteAComment(userId, CommentID);
            }
            return CommentID;
        }

        public ActionResult AddNewPhoto()
        {
            var photoContent = TempData["FileInfoList"];
            var destinationId = new Guid(TempData["MasterID"].ToString());
            var photoInfo = (Content) TempData["MediaContent"];
            //add Content here
            var userId = new Guid(User.Identity.GetUserId());
            destinationService.AddNewContent(photoInfo, destinationId, userId);
            //delete temporary data
            TempData.Remove("FileInfoList");
            TempData.Remove("MasterID");
            TempData.Remove("MediaContent");
            return Json(photoContent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LikeUnlike(Guid userID, Guid destinationID)
        {
            string result;

            if (destinationService.UserAlreadyLike(userID, destinationID))
            {
                destinationService.UnlikeDestination(userID, destinationID);
            }
            else
            {
                destinationService.LikeDestination(userID, destinationID);
            }

            result = "Success";
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
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
        public ActionResult DestinationMainContentWidget(DestinationViewModel viewModel)
        {
            return PartialView("DestinationMainContentWidget", viewModel);
        }

             
        public ActionResult ShareDestination(Guid userID, Guid destinationID, string content)
        {
            var result = "Fail";
            destinationService.ShareDestination(userID, destinationID, content);

            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
