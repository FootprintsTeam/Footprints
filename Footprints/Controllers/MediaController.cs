﻿using Footprints.Common;
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
        public MediaController(IDestinationService destinationService)
        {
            this.destinationService = destinationService;
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
        public ActionResult Albums()
        {
            var model = AlbumsViewModel.GetSampleObject();
            return View(model);
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
            albumDetails.AlbumId = destination.AlbumID;
            albumDetails.AlbumName = destination.Name;
            albumDetails.DestinationName = destination.Name;
            albumDetails.JourneyId = destination.JourneyID;
            

            return View(AlbumDetailsViewModel.GetSampleObject());
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
