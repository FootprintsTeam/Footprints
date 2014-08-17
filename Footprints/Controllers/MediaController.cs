using Footprints.Common;
using Footprints.Common.JsonModel;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Footprints.Services;
using Footprints.Models;
using System.IO;
using System.Text;

namespace Footprints.Controllers
{
    public class MediaController : Controller
    {
        int NumberOfPhotoPerLoad = 20;
        int NumberOfAlbumPerload = 4;

        IDestinationService destinationService;
        IJourneyService journeyService;
        IUserService userService;
        
        public MediaController(IDestinationService destinationService, IJourneyService journeyService, IUserService userService)
        {
            this.destinationService = destinationService;
            this.journeyService = journeyService;
            this.userService = userService;
        }
        //
        // GET: /Media/
        public ActionResult Index(string userID = "default")
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            Guid currentUserID = new Guid(User.Identity.GetUserId());
            Guid targetUserID = userID.Equals("default") ? currentUserID :  regex.IsMatch(userID) ? new Guid(userID) : currentUserID;
            var numberOfContent = userService.GetNumberOfContentByUserID(targetUserID);
            
            IList<Content> contentList;
            if (numberOfContent > 0)
            {
                contentList = userService.GetListContentByUserID(targetUserID, 0, NumberOfPhotoPerLoad);
            }
            else
            {
                contentList = new List<Content>();
            }
            var model = new MediaViewModel
            {
                Photos = contentList,
                NumberOfPhotos = numberOfContent,
                TargetUserID = targetUserID
            };
            return View(model);
        }
        public ActionResult Albums(string userID = "default")
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            Guid targetUserID = new Guid(userID.Equals("default") ? User.Identity.GetUserId() : regex.IsMatch(userID) ? userID : User.Identity.GetUserId());
            var journeyList = userService.GetJourneyThumbnailWithSkipLimit(targetUserID, 0, NumberOfAlbumPerload);
            var albumsViewModel = new AlbumsViewModel();
            albumsViewModel.AlbumList = new List<AlbumDetailsViewModel>();
            albumsViewModel.TargetUserID = targetUserID;
            if (journeyList != null && journeyList.Count > 0)
            {
                AlbumDetailsViewModel albumDetailsViewModel;
                foreach (var journey in journeyList)
                {
                    foreach (var destination in journey.Destinations)
                    {
                        albumDetailsViewModel = new AlbumDetailsViewModel();
                        albumDetailsViewModel.DestinationID = destination.DestinationID;
                        albumDetailsViewModel.DestinationName = destination.Name;
                        albumDetailsViewModel.JourneyID = journey.JourneyID;
                        albumDetailsViewModel.JourneyName = journey.Name;
                        albumDetailsViewModel.Photos = destinationService.GetContentListWithSkipAndLimit(0, 4, destination.DestinationID);
                        albumDetailsViewModel.NumberOfPhotos = destinationService.GetNumberOfContentInDestination(albumDetailsViewModel.DestinationID);
                        if (albumDetailsViewModel.Photos != null && albumDetailsViewModel.Photos.Count > 0)
                        {
                            albumsViewModel.NumberOfPhotos += albumDetailsViewModel.NumberOfPhotos;
                            albumsViewModel.AlbumList.Add(albumDetailsViewModel);
                        }
                    }
                }
            }
            return View(albumsViewModel);
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

        public ActionResult AlbumDetails(Guid DestinationID, Guid AlbumID)
        {
            Guid defaultGuid = new Guid();
            var destination = destinationService.GetDestinationDetail(DestinationID);
            if (destination == null || destination.DestinationID == defaultGuid)
            {
                return RedirectToAction("Index", "Media");
            }
            AlbumDetailsViewModel albumDetails = new AlbumDetailsViewModel();
            albumDetails.AlbumID = destination.AlbumID;
            albumDetails.DestinationName = destination.Name;
            albumDetails.JourneyID = destination.JourneyID;
            var journey = journeyService.GetJourneyDetail(destination.JourneyID);
            albumDetails.JourneyName = journey.Name;
            albumDetails.Photos = destination.Contents;
            return View(albumDetails);
        }

        public ActionResult CreateAlbum()
        {
            return View();
        }

        public ActionResult DeletePhoto(Guid id)
        {
            System.Diagnostics.Debug.WriteLine("PhotoID - id = [" + id.ToString() + "]");
            return View();
        }

        public ActionResult LazyLoadAllPhoto(String userID, int BlockNumber)
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            if (userID == null || userID.Length == 0 || BlockNumber <= 0 || !regex.IsMatch(userID)) return null;
            var targetUserID = new Guid(userID);
            Guid currentUserID = new Guid(User.Identity.GetUserId());

            IList<Content> contentList = userService.GetListContentByUserID (targetUserID, BlockNumber * NumberOfPhotoPerLoad, NumberOfPhotoPerLoad);
            InfiniteScrollPhotoListJsonModel jsonModel = new InfiniteScrollPhotoListJsonModel();
            jsonModel.HTMLString = "";
            jsonModel.NoMoreData = true;
            jsonModel.PhotoList = new List<string>();
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
            return Json(jsonModel);
        }

        public ActionResult LazyLoadAlbums(string userID, int BlockNumber)
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            if (userID == null || userID.Length == 0 || BlockNumber <= 0 || !regex.IsMatch(userID)) return null;
            var targetUserID = new Guid(userID);
            var journeyList = userService.GetJourneyThumbnailWithSkipLimit(targetUserID, BlockNumber * NumberOfAlbumPerload, NumberOfAlbumPerload);
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();
            jsonModel.HTMLString = "";
            jsonModel.NoMoreData = true;

            if (journeyList != null && journeyList.Count > 0)
            {
                AlbumDetailsViewModel albumDetailsViewModel;
                StringBuilder sbReturnHtml = new StringBuilder("");
                try
                {
                    int albumCount = 0;
                    foreach (var journey in journeyList)
                    {
                        foreach (var destination in journey.Destinations)
                        {
                            albumDetailsViewModel = new AlbumDetailsViewModel();
                            albumDetailsViewModel.DestinationID = destination.DestinationID;
                            albumDetailsViewModel.DestinationName = destination.Name;
                            albumDetailsViewModel.JourneyID = journey.JourneyID;
                            albumDetailsViewModel.JourneyName = journey.Name;
                            albumDetailsViewModel.Photos = destinationService.GetContentListWithSkipAndLimit(0, 4, destination.DestinationID);
                            if (albumDetailsViewModel.Photos != null && albumDetailsViewModel.Photos.Count > 0)
                            {
                                albumDetailsViewModel.NumberOfPhotos = albumDetailsViewModel.Photos.Count();
                                sbReturnHtml.Append(RenderPartialViewToString("GalleryWidget", albumDetailsViewModel));
                            }
                            albumCount++;
                        }
                    }
                    jsonModel.HTMLString = sbReturnHtml.ToString();
                    if (albumCount >= NumberOfAlbumPerload)
                    {
                        jsonModel.NoMoreData = false;
                    }
                }
                catch { }
            }
            return Json(jsonModel);
        }

        [ChildActionOnly]
        public PartialViewResult GalleryWidget()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult PhotoList()
        {
            return PartialView();
        }


    }
}
