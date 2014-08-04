﻿using AutoMapper;
using Footprints.Services;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace Footprints.Controllers
{
    public class DestinationController : Controller
    {
        IDestinationService destinationService;
        public DestinationController(IDestinationService destinationService)
        {
            this.destinationService = destinationService;
        }

        //
        // GET: /Destination/
        public ActionResult Index()
        {
            var model = Footprints.ViewModels.DestinationViewModel.GetSampleObject();
            return View(model);
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
            var place = Mapper.Map<AddNewDestinationFormViewModel, Models.Place>(model);
            System.Diagnostics.Debug.WriteLine("Place: = ["+place.ToString()+"]");
            var destination = Mapper.Map<AddNewDestinationFormViewModel, Models.Destination>(model);
            System.Diagnostics.Debug.WriteLine("destination: = [" + destination.ToString() + "]");
            destinationService.AddNewDestination(new Guid(), destination, place, model.JourneyID);
            return View();
        }


        //
        // POST: /Destination/Edit/5
        [HttpPost]
        public ActionResult Edit(AddNewDestinationFormViewModel model)
        {
            //destinationService.UpdateDestination(Mapper.Map<AddNewDestinationFormViewModel, Models.Destination>(model));
            return View();
        }

        //
        // GET: /Destination/Delete/5
        //[Authorize]
        public ActionResult Delete(Guid id, Guid JourneyID)
        {
            var user = Membership.GetUser(User.Identity.Name);
            Guid userId = (Guid)user.ProviderUserKey;
            System.Diagnostics.Debug.WriteLine("Userid = [" + userId + "]");
            destinationService.DeleteDestination(userId, id);
            RedirectToAction("Index", "Journey", new { id = JourneyID });
            //Redirect to Journey
            return View();
        }

        [HttpPost]
        public ActionResult Comment(CommentViewModel comment){
            var data = new List<CommentViewModel>();
            data.Add(comment);
             return Json(data, JsonRequestBehavior.DenyGet);
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
    }
}
