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
            model.JourneyID = new Guid("54088c27-11b8-4c3a-8919-d86c5620964b");
            return View("Index", model);
        }
        //
        // GET: /Journey/
        public ActionResult Index(string username, Guid journeyID)
        {
            var journeyModel = journeyService.GetJourneyDetail(journeyID);
            var journeyViewModel = Mapper.Map<Journey, JourneyViewModel>(journeyModel);
            if (journeyModel.Destinations != null)
            {
                foreach (Destination d in journeyModel.Destinations)
                {
                    journeyViewModel.Destinations.Add(Mapper.Map<Destination, DestinationViewModel>(d));
                }
            }
            journeyViewModel.AddNewDestinationFormViewModel = new AddNewDestinationFormViewModel { JourneyID = journeyID, TakenDate = DateTimeOffset.Now };
            return View(journeyViewModel);
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

        //
        // POST: /Journey/Edit/5
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
        public ActionResult Delete(FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var jouneyID = new Guid(Request.Form["JourneyID"] as string);
                var result = journeyService.DeleteJourney(new Guid(User.Identity.GetUserId()), jouneyID);
                if (result) { 
                return Content("success");
                } else return Content ("fail");
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
