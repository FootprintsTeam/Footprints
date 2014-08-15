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
        public ActionResult AllPhotos()
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Albums(Guid userID)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Albums", "Media", new { userID = User.Identity.GetUserId()});
            }
            var journeyList = userService.GetJourneyThumbnail(userID);
            var albumsViewModel = new AlbumsViewModel();
            if (journeyList != null && journeyList.Count > 0)
            {
                AlbumDetailsViewModel albumDetailsViewModel;
                foreach (var journey in journeyList)
                {
                    albumsViewModel.AlbumList = new List<AlbumDetailsViewModel>();
                    foreach (var destination in journey.Destinations)
                    {
                        albumDetailsViewModel = new AlbumDetailsViewModel();
                        albumDetailsViewModel.DestinationID = destination.DestinationID;
                        albumDetailsViewModel.DestinationName = destination.Name;
                        albumDetailsViewModel.JourneyID = journey.JourneyID;
                        albumDetailsViewModel.JourneyName = journey.Name;
                        albumDetailsViewModel.Photos = destinationService.GetAllContent(destination.DestinationID);
                        if (albumDetailsViewModel.Photos != null)
                        albumDetailsViewModel.NumberOfPhotos = albumDetailsViewModel.Photos.Count();
                        albumsViewModel.AlbumList.Add(albumDetailsViewModel);
                    }
                }
                
            }
            return View(albumsViewModel);
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
    }
}
