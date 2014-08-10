using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Services;
using Footprints.Models;
using System.Net;
using Microsoft.AspNet.Identity;

namespace Footprints.Controllers
{
    public class AdminController : Controller
    {
        public IUserService userSer;
        public IJourneyService journeySer;
        public IDestinationService destinationSer;
        public AdminController(IUserService userSer ,IJourneyService journeySer, IDestinationService destinationSer)
        {
            this.userSer = userSer;
            this.journeySer = journeySer;
            this.destinationSer = destinationSer;
        }
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Journey()
        {
            IList<Journey> list = journeySer.GetAllJourney();
            return View(list);
        }

        public ActionResult Destination() {
            IList<Destination> list = destinationSer.GetAllDestination();
            return View(list);
        }
        
        public ActionResult DeleteJourney(Guid UserID, Guid JourneyID) {            
            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);                
            }
            else if (JourneyID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else {
                journeySer.DeleteJourney(UserID, JourneyID);                
                System.Diagnostics.Debug.WriteLine("Deleted");
                return RedirectToAction("Journey");
            }
            
        }

        public ActionResult EditJourney(Journey Journey) {
            if (Journey.JourneyID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journey JourneyRetrieved = journeySer.RetrieveJourney(Journey.JourneyID);
            return View(JourneyRetrieved);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJourney(Guid UserID, Journey Journey) {
            if (ModelState.IsValid) {
                journeySer.UpdateJourney(UserID, Journey);
            }
            return RedirectToAction("Journey");
        }
    }
}