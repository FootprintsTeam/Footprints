using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Service;
using Neo4jClient;

namespace Footprints.Areas.Destination.Controllers
{
    public class DestinationController : Controller
    {
        IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }
        //
        // GET: /Destination/Destination/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Destination/Destination/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Destination/Destination/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Destination/Destination/Create
        [HttpPost]
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
        // GET: /Destination/Destination/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Destination/Destination/Edit/5
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
        // GET: /Destination/Destination/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Destination/Destination/Delete/5
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
