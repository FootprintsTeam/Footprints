using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.Services;
using Footprints.Models;
using System.Net;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Footprints.Controllers
{
    public class AdminController : Controller
    {
        public const int pageSize = 10;

        public IUserService userSer;
        public IJourneyService journeySer;
        public IDestinationService destinationSer;
        public AdminController(IUserService userSer, IJourneyService journeySer, IDestinationService destinationSer)
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

        public ActionResult Destination()
        {
            IList<Destination> list = destinationSer.GetAllDestination();
            return View(list);
        }

        public ActionResult DeleteDestination(Guid UserID, Guid DestinationID)
        {
            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (DestinationID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                destinationSer.DeleteDestination(UserID, DestinationID);
                return RedirectToAction("Destination");
            }
        }

        public ActionResult EditDestination(Guid DestinationID) {
            if (DestinationID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination DestinationRetrieved = destinationSer.GetDestinationDetail(DestinationID);
            return View(DestinationRetrieved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDestination(Destination Destination)
        {
            if (ModelState.IsValid)
            {
                //destinationSer.UpdateDestination(Destination);
                return RedirectToAction("Destination");
            }
            return View(Destination);
        }

        public ActionResult UserList()
        {
            IList<User> list = userSer.GetUser();
            return View(list);
        }

        public ActionResult DeleteUser(Guid UserID)
        {
            //Guid CurrentAdminID = new Guid(User.Identity.GetUserId());
            Guid CurrentAdminID = new Guid("5BBE3A24-99A2-4DE5-85B9-FF8599CF26CD");
            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check if admin wanna delete himself
            else if (UserID == CurrentAdminID)
            {
                ModelState.AddModelError("", "Cannot delete your Admin account");
                return RedirectToAction("UserList");
            }
            else
            {
                userSer.DeleteUser(UserID);
                return RedirectToAction("UserList");
            }
        }

        public ActionResult Journey(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<Journey> list = journeySer.GetAllJourney();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteJourney(Guid UserID, Guid JourneyID)
        {
            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (JourneyID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                journeySer.DeleteJourney(UserID, JourneyID);
                return RedirectToAction("Journey");
            }

        }

        public ActionResult EditJourney(Guid JourneyID)
        {
            if (JourneyID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journey JourneyRetrieved = journeySer.RetrieveJourney(JourneyID);
            return View(JourneyRetrieved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJourney(Journey Journey)
        {
            if (ModelState.IsValid)
            {
                journeySer.UpdateJourneyForAdmin(Journey);
                return RedirectToAction("Journey");
            }
            return View(Journey);
        }
    }
}