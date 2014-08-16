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
        public ActionResult Index()
        {
            var model = MediaViewModel.GetSampleObject();
            return View(model);
        }
        public ActionResult AllPhotos(string userID = "default")
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            Guid targetUserID = new Guid(userID.Equals("default") ? User.Identity.GetUserId() : regex.IsMatch(userID) ? userID : User.Identity.GetUserId());
            return View();
        }
        public ActionResult Albums(string userID = "default")
        {
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            Guid targetUserID = new Guid(userID.Equals("default") ? User.Identity.GetUserId() : regex.IsMatch(userID) ? userID : User.Identity.GetUserId());
            var journeyList = userService.GetJourneyThumbnail(targetUserID);
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
                        if (albumDetailsViewModel.Photos != null && albumDetailsViewModel.Photos.Count > 0)
                        {
                            albumDetailsViewModel.NumberOfPhotos = albumDetailsViewModel.Photos.Count();
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

        public ActionResult LazyLoadAlbums(string userID, int BlockNumber)
        {
            int nAlbumPerBlock = 4;
            Regex regex = new Regex(Common.Constant.GUID_REGEX);
            if (userID == null || userID.Length == 0 || BlockNumber <= 0 || !regex.IsMatch(userID)) return null;
            var targetUserID = new Guid(userID);
            var journeyList = userService.GetJourneyThumbnailWithSkipLimit(targetUserID, BlockNumber * nAlbumPerBlock, (BlockNumber + 1) * nAlbumPerBlock);
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();

            jsonModel.HTMLString = "";
            jsonModel.NoMoreData = true;
            if (journeyList != null && journeyList.Count > 0)
            {
                AlbumDetailsViewModel albumDetailsViewModel;
                StringBuilder sbReturnHtml = new StringBuilder("");
                try
                {
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
                        }
                    }
                    jsonModel.HTMLString = sbReturnHtml.ToString();
                    jsonModel.NoMoreData = false;
                }
                catch { }
            }
            return Json(jsonModel);
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

        [ChildActionOnly]
        public ActionResult GalleryWidget()
        {
            return View();
        }
    }
}
