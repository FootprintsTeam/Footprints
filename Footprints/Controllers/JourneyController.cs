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
namespace Footprints.Controllers
{
    public class JourneyController : Controller
    {
        IJourneyService journeyService;
        IDestinationService destinationService;
        IUserService userService;
        public JourneyController(IJourneyService journeyService, IDestinationService destinationService, IUserService userService)
        {
            this.journeyService = journeyService;
            this.destinationService = destinationService;
            this.userService = userService;
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
        public ActionResult Index(string username, Guid journeyID)
        {
            var journeyModel = journeyService.GetJourneyDetailWithComment(journeyID);
            //Implementing
            //Journey does not exist
            if (journeyModel == null)
            {
                //Redirect to error page or newsfeed page
                return RedirectToAction("Index", "Newsfeed");
            }
            var journeyViewModel = Mapper.Map<Journey, JourneyViewModel>(journeyModel);
            var journeyOwner = userService.RetrieveUser(journeyViewModel.UserID);
            journeyViewModel.NumberOfDestination = journeyViewModel.Destinations.Count();
            journeyViewModel.NumberOfLike = journeyService.GetNumberOfLike(journeyID);
            journeyViewModel.NumberOfShare = journeyService.GetNumberOfShare(journeyID);
            foreach (var x in journeyViewModel.Destinations)
            {
                Mapper.Map<User, DestinationViewModel>(journeyOwner, x);
            }
            if (journeyViewModel.Comments == null)
            {
                journeyViewModel.Comments = new List<CommentViewModel>();
            }
            journeyViewModel.AddNewDestinationFormViewModel = new AddNewDestinationFormViewModel { JourneyID = journeyID, TakenDate = DateTimeOffset.Now };
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
    }
}
