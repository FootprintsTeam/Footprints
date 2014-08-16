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
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace Footprints.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        public const int pageSize = 10;
        public IUserService userSer;
        public IJourneyService journeySer;
        public IDestinationService destinationSer;

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public AdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public AdminController(IUserService userSer, IJourneyService journeySer, IDestinationService destinationSer)
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
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

        public ActionResult Destination(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<Destination> list = destinationSer.GetAllDestination();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteDestination(Guid DestinationID)
        {
            if (DestinationID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                destinationSer.DeleteDestinationForAdmin(DestinationID);
                return RedirectToAction("Destination");
            }
        }

        public ActionResult EditDestination(Guid DestinationID)
        {
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
                destinationSer.UpdateDestinationForAdmin(Destination);
                return RedirectToAction("Destination");
            }
            return View(Destination);
        }

        public ActionResult UserList(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<User> list = userSer.GetUser();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteUser(Guid UserID)
        {
            Guid CurrentAdminID = new Guid(User.Identity.GetUserId());
            var user = UserManager.FindById(UserID.ToString());

            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check if admin wanna delete himself
            else if (UserID == CurrentAdminID)
            {
                ModelState.AddModelError("", "Cannot delete your Admin account");
                return View();
            }
            else if (user != null)
            {
                userSer.DeleteUser(UserID);
                UserManager.DeleteAsync(user);
            }
            return RedirectToAction("UserList");
        }

        public ActionResult EditUser(Guid UserID)
        {
            if (UserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footprints.Models.User UserRetrieved = userSer.RetrieveUser(UserID);
            return View(UserRetrieved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User User)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.SetEmail(User.UserID.ToString(), User.Email.ToString());
                if (user != null)
                {
                    userSer.UpdateUser(User);
                    return RedirectToAction("UserList");
                }
            }
            return View(User);
        }

        public ActionResult BanUser(Guid UserID)
        {
            var roleResult = UserManager.AddToRole(UserID.ToString(), "Banned");
            if (roleResult != null)
            {
                userSer.BanUser(UserID);
                return RedirectToAction("UserList");
            }
            return View(userSer.RetrieveUser(UserID));
        }

        public ActionResult GrantAdminPermision(Guid UserID)
        {
            var roleResult = UserManager.AddToRole(UserID.ToString(), "Administrator");
            if (roleResult != null)
            {
                userSer.GrantAdminPrivilege(UserID);
                return RedirectToAction("UserList");
            }
            return View(userSer.RetrieveUser(UserID));
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