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
using Footprints.Models;

namespace Footprints.Controllers
{
    public class DestinationController : Controller
    {
        IDestinationService destinationService;
        ICommentService commentService;
        IUserService userService;
        public DestinationController(IDestinationService destinationService, ICommentService commentService)
        {
            this.destinationService = destinationService;
            this.commentService = commentService;
        }

        //
        // GET: /Destination/
        public ActionResult Index()
        {
            var model = Footprints.ViewModels.DestinationViewModel.GetSampleObject();
            return View(model);
        }

        public ActionResult Index(Guid destinationID) {
            var destinationModel = destinationService.GetDestination(destinationID);
            var comments = commentService.RetrieveDestinationComment(destinationID);
            //implementing
            return View();
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
            return RedirectToAction("Index","Destination",destination.DestinationID);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(CommentViewModel comment)
        {
            var commentObj = (Models.Comment)Mapper.Map<CommentViewModel, Models.Comment>(comment);
            ////reset timestamp to current
            //commentObj.Timestamp = DateTimeOffset.Now;
            //commentService.UpdateComment(commentObj);
            ////return Json(comment, JsonRequestBehavior.DenyGet);
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

        public string LikeUnlike(Guid userID, Guid destinationID) { 
            return "Success";
        }

        [ChildActionOnly]
        public ActionResult EditCommentForm()
        {
            return View();
        }
    }
}
