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
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
