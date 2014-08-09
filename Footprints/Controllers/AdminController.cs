using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Services;
using Footprints.Models;
using System.Net;

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

        public ActionResult ViewJourney()
        {
            IList<Journey> list = journeySer.GetAllJourney();
            return View(list);
        }

        public ActionResult ViewDestination() {
            IList<Destination> list = destinationSer.GetAllDestination();
            return View(list);
        }
        
        public ActionResult DeleteJourney(Guid UserID, Guid JourneyID) {
            System.Diagnostics.Debug.WriteLine("Start Delete");
           
            if ((UserID == null) && (JourneyID ==null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);                
            }
            else {
                journeySer.DeleteJourney(UserID, JourneyID);
                System.Diagnostics.Debug.WriteLine("Deleted");
                return RedirectToAction("ViewJourney");
            }                       
        }
    }
}