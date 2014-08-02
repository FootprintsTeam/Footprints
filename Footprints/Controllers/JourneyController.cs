using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;
using Footprints.Services;

namespace Footprints.Controllers
{
    public class JourneyController : Controller
    {
        IJourneyService journeyService;
        public JourneyController(IJourneyService journeyService) {
            this.journeyService = journeyService;
        }
        //
        // GET: /Journey/
        public ActionResult Index(Guid id)
        {
            var model = JourneyViewModel.GetSampleObject(id);
            return View(model);
        }

        //
        // GET: /Journey/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Journey/Create
        public ActionResult Create()
        {            
            return View();
        }

        //
        // POST: /Journey/Create
        [HttpPost]
        public ActionResult Create(AddNewJourneyViewModel journey)
        {
            try
            {
                // TODO: Add insert logic here
                var viewModel = journey;
                journeyService.AddJourney(viewModel);                
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
