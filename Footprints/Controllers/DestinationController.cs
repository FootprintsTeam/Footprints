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
using Footprints.Common.JsonModel;
using System.IO;
namespace Footprints.Controllers
{
    public class DestinationController : Controller
    {
        int NumberOfPhotoPerLoad = 18;

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
            var destinationModel = destinationService.GetDestinationDetailWithLimitedContent(destinationID, NumberOfPhotoPerLoad);
            var destinationViewModel = Mapper.Map<Destination, DestinationViewModel>(destinationModel);
            var comments = commentService.RetrieveDestinationComment(destinationID);
            if (comments != null && comments.Count > 0)
                destinationViewModel.Comments = comments.ToList<Comment>();
            destinationViewModel.NumberOfJourney = journeyService.GetJourneyListBelongToUser(destinationModel.UserID).Count;
            destinationViewModel.NumberOfDestination = destinationService.GetNumberOfDestination(destinationModel.UserID);
            destinationViewModel.NumberOfFriend = (int)userService.GetNumberOfFriend(destinationModel.UserID);
            destinationViewModel.EditDestinationForm = new EditDestinationFormViewModel(destinationViewModel.Place);
            destinationViewModel.EditDestinationForm = Mapper.Map<DestinationViewModel, EditDestinationFormViewModel>(destinationViewModel);
            Mapper.Map<User, DestinationViewModel>(userService.RetrieveUser(destinationViewModel.UserID), destinationViewModel);
            //check if user already like or share
            TempData["AlreadyLike"] = destinationService.UserAlreadyLike(userId, destinationID);
            TempData["AlreadyShare"] = destinationService.UserAlreadyShared(userId, destinationID);
            TempData["IsAuthor"] = userId == destinationModel.UserID ? true : false;
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
            var maxOrderNumber = destinationService.GetMaxOrderNumber(model.JourneyID);
            destination.OrderNumber = ++maxOrderNumber;
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
                return null;
            }
            var userId = new Guid(User.Identity.GetUserId());
            var destination = destinationService.GetDestination(model.DestinationID);

            if (userId == destination.UserID)
            {
                List<String> listContentUrl = new List<String>();
                IList<Content> contents = destinationService.GetAllContent(destination.DestinationID);
                if (contents != null && contents.Count > 0)
                {
                    foreach (var item in contents)
                    {
                        listContentUrl.Add("/" + item.ContentID + ".jpg");
                        listContentUrl.Add("/thumbnails/" + item.ContentID + ".jpg");
                    }
                }
                listContentUrl.Add("/thumbnails");
                listContentUrl.Add("");
                ImageProcessor.DeleteAlbums(userId.ToString(), destination.AlbumID.ToString(), listContentUrl);
            }
            destinationService.DeleteDestination(userId, model.DestinationID);
            return RedirectToAction("Index", "Journey", new { journeyID = model.JourneyID });
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
            if (commentService.AddDestinationComment(userId, commentObj))
            {
                if (Request.UrlReferrer.ToString().Contains("/Journey/Index"))
                {
                    TempData.Add("CommentPage", "Journey");
                }
                else if (Request.UrlReferrer.ToString().Contains("/Newsfeed/Index"))
                {
                    TempData.Add("CommentPage", "Newsfeed");
                }
                var user = userService.RetrieveUser(userId);
                commentObj.User = user;
                jsonModel.HTMLString = RenderPartialViewToString("CommentItem", commentObj);
                TempData.Remove("CommentPage");
            }
            else
            {
                jsonModel.HTMLString = "";
            }
            jsonModel.DestinationID = comment.DestinationID;
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
            if (!ModelState.IsValid)
            {
                return null;
            }
            var userId = new Guid(User.Identity.GetUserId());
            var commentObj = (Models.Comment)Mapper.Map<CommentViewModel, Models.Comment>(comment);
            //reset timestamp to current
            commentObj.Timestamp = DateTimeOffset.Now;
            //reset number of like
            IEnumerable<User> userLikedList = destinationService.GetAllUserLiked(comment.DestinationID);
            commentObj.NumberOfLike = userLikedList == null ? 0 : userLikedList.Count();
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewPhoto(Guid DestinationID)
        {
            FileInfoList fileInforList = null;
            if (Request.Files.Count > 0)
            {
                var UserID = new Guid(User.Identity.GetUserId());
                var defaultGuid = new Guid();
                var destination = destinationService.GetDestination(DestinationID);
                if (destination == null || destination.DestinationID == defaultGuid || destination.UserID != UserID)
                {
                    return null;
                }
                var ContentID = Guid.NewGuid();
                string deleteUrl = Url.Action("DeletePhoto", "Media", new { id = ContentID });
                fileInforList = ImageProcessor.UploadPhoto(UserID.ToString(), destination.AlbumID.ToString(), ContentID.ToString(), Request.Files.Get(0).InputStream, deleteUrl);
                if (fileInforList.files[0].error == null)
                {
                    Content uploadedPhoto = new Content
                    {
                        ContentID = ContentID,
                        Timestamp = DateTimeOffset.Now,
                        URL = fileInforList.files[0].url,
                        ThumbURL = fileInforList.files[0].thumbnailUrl
                    };
                    destinationService.AddNewContent(uploadedPhoto, DestinationID, UserID);
                }
            }
            return Json(fileInforList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhoto(DeletePhotoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            var userID = new Guid(User.Identity.GetUserId());
            var destination = destinationService.GetDestination(model.DestinationID);
            if (userID == destination.UserID)
            {
                destinationService.DeleteContent(userID, model.ContentID);
                ImageProcessor.DeletePhoto(userID.ToString(), destination.AlbumID.ToString(), model.ContentID.ToString());
                return Json(new { Result = "success", ContentID = model.ContentID }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { Result = "fail", ContentID = model.ContentID }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult LikeUnlike(Guid destinationID)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Result = "fail" }, JsonRequestBehavior.AllowGet);
            }
            string result;
            var userID = new Guid(User.Identity.GetUserId());
            if (destinationService.UserAlreadyLike(userID, destinationID))
            {
                destinationService.UnlikeDestination(userID, destinationID);
            }
            else
            {
                destinationService.LikeDestination(userID, destinationID);
            }
            int numberOfLike = destinationService.GetNumberOfLike(destinationID);
            result = "Success";
            return Json(new { Result = result, NumberOfLike = numberOfLike }, JsonRequestBehavior.AllowGet);
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
        public ActionResult ShareDestination(Guid userID, Guid destinationID, string content = "content")
        {
            var result = "Success";
            destinationService.ShareDestination(userID, destinationID, content);

            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LazyLoadGallery(int BlockNumber, Guid destinationID)
        {
            InfiniteScrollPhotoListJsonModel jsonModel = new InfiniteScrollPhotoListJsonModel();
            jsonModel.NoMoreData = true;
            IList<Content> contentList = destinationService.GetContentListWithSkipAndLimit(BlockNumber * NumberOfPhotoPerLoad, NumberOfPhotoPerLoad, destinationID);
            var destination = destinationService.GetDestination(destinationID);
            var userId = new Guid(User.Identity.GetUserId());
            jsonModel.PhotoList = new List<string>();
            TempData["IsAuthor"] = userId == destination.UserID ? true : false;
            if (contentList != null && contentList.Count() > 0)
            {
                foreach (var content in contentList)
                {
                    jsonModel.PhotoList.Add(content.ContentID.ToString());
                }
                jsonModel.HTMLString = RenderPartialViewToString("PhotoList", contentList);
                if (contentList.Count >= NumberOfPhotoPerLoad)
                {
                    jsonModel.NoMoreData = false;
                }
            }
            TempData.Remove("IsAuthor");
            return Json(jsonModel);
        }
    }
}
