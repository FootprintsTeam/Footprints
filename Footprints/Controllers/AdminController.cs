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
    [Authorize(Roles = "Admin")]
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

        public ActionResult SearchDestination(String keyword)
        {
            IList<Destination> list = destinationSer.GetAllDestination();
            var result = list.Where(u => u.Name.Contains(keyword)).OrderBy(u => u.Name).ToList();
            return PartialView(result);
        }

        public ActionResult DeleteDestination(Guid DestinationID)
        {
            if (DestinationID == null)
            {
                TempData["Msg"] = "Delete destination failed";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                destinationSer.DeleteDestinationForAdmin(DestinationID);
                TempData["Msg"] = "Delete destination successfully";
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
                TempData["Msg"] = "Destination has been updated successfully";
                return RedirectToAction("Destination");
            }
            else
            {
                TempData["Msg"] = "Update destination failed";
                ModelState.AddModelError("", "Error");
            }
            return View(Destination);
        }

        //public static IList<User> list;
        public ActionResult UserList(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<User> list = userSer.GetUser();
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SearchUser(String keyword)
        {
            IList<User> list = userSer.GetUser();
            var result = list.Where(u => u.UserName.Contains(keyword)).OrderBy(u=> u.UserName).ToList();           
            return PartialView(result);
        }

        public ActionResult DeleteUser(Guid UserID)
        {
            Guid CurrentAdminID = new Guid(User.Identity.GetUserId());
            var user = UserManager.FindById(UserID.ToString());

            if (UserID == null)
            {
                TempData["Msg"] = "Can not delete this account !";
            }
            //check if admin wanna delete himself
            else if (UserID == CurrentAdminID)
            {
                TempData["Msg"] = "You can not delete your account";
            }
            else if (user != null)
            {
                IdentityResult result = UserManager.Delete(user);
                if (result.Succeeded)
                {
                    userSer.DeleteUser(UserID);
                    TempData["Msg"] = "Delete user " + user.UserName + " successfully";
                }
                else { TempData["Msg"] = "Delete user failed"; }
            }
            else
            {
                TempData["Msg"] = "Can not delete this account !";
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
                IdentityResult user = UserManager.SetEmail(User.UserID.ToString(), User.Email.ToString());
                if (user.Succeeded)
                {
                    if (User.Status.Equals(Footprints.Models.StatusEnum.Active))
                    {
                        Footprints.Models.User uActive = new Models.User
                        {
                            UserID = User.UserID,
                            About = User.About,
                            FirstName = User.FirstName,
                            LastName = User.LastName,
                            Email = User.Email,
                            Address = User.Address,
                            PhoneNumber = User.PhoneNumber,
                            Genre = User.Genre,
                            DateOfBirth = User.DateOfBirth,
                            UserName = User.UserName,
                            Status = StatusEnum.Active,
                            JoinDate = User.JoinDate,
                            ProfilePicURL = User.ProfilePicURL,
                            CoverPhotoURL = User.CoverPhotoURL,
                        };
                        IdentityResult roleResult = UserManager.AddToRole(User.UserID.ToString(), "Active");
                        if (roleResult.Succeeded)
                        {
                            userSer.UpdateUser(uActive);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                    }
                    else if (User.Status.Equals(Footprints.Models.StatusEnum.Admin))
                    {
                        Footprints.Models.User uActive = new Models.User
                        {
                            UserID = User.UserID,
                            About = User.About,
                            FirstName = User.FirstName,
                            LastName = User.LastName,
                            Email = User.Email,
                            Address = User.Address,
                            PhoneNumber = User.PhoneNumber,
                            Genre = User.Genre,
                            DateOfBirth = User.DateOfBirth,
                            UserName = User.UserName,
                            Status = StatusEnum.Admin,
                            JoinDate = User.JoinDate,
                            ProfilePicURL = User.ProfilePicURL,
                            CoverPhotoURL = User.CoverPhotoURL,
                        };

                        IdentityResult roleResult = UserManager.AddToRole(User.UserID.ToString(), "Admin");
                        if (roleResult.Succeeded)
                        {
                            userSer.UpdateUser(uActive);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                    }
                    else if (User.Status.Equals(Footprints.Models.StatusEnum.Banned))
                    {
                        Footprints.Models.User uActive = new Models.User
                        {
                            UserID = User.UserID,
                            About = User.About,
                            FirstName = User.FirstName,
                            LastName = User.LastName,
                            Email = User.Email,
                            Address = User.Address,
                            PhoneNumber = User.PhoneNumber,
                            Genre = User.Genre,
                            DateOfBirth = User.DateOfBirth,
                            UserName = User.UserName,
                            Status = StatusEnum.Banned,
                            JoinDate = User.JoinDate,
                            ProfilePicURL = User.ProfilePicURL,
                            CoverPhotoURL = User.CoverPhotoURL,
                        };
                        IdentityResult roleResult = UserManager.AddToRole(User.UserID.ToString(), "Banned");
                        if (roleResult.Succeeded)
                        {
                            userSer.UpdateUser(uActive);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                    }
                    else if (User.Status.Equals(Footprints.Models.StatusEnum.Inactive))
                    {
                        Footprints.Models.User uActive = new Models.User
                        {
                            UserID = User.UserID,
                            About = User.About,
                            FirstName = User.FirstName,
                            LastName = User.LastName,
                            Email = User.Email,
                            Address = User.Address,
                            PhoneNumber = User.PhoneNumber,
                            Genre = User.Genre,
                            DateOfBirth = User.DateOfBirth,
                            UserName = User.UserName,
                            Status = StatusEnum.Inactive,
                            JoinDate = User.JoinDate,
                            ProfilePicURL = User.ProfilePicURL,
                            CoverPhotoURL = User.CoverPhotoURL,
                        };
                        IdentityResult roleResult = UserManager.AddToRole(User.UserID.ToString(), "Inactive");
                        if (roleResult.Succeeded)
                        {
                            userSer.UpdateUser(uActive);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                    }

                }
                else
                {
                    TempData["Msg"] = "Update user information failed !";
                    ModelState.AddModelError("", user.ToString());
                }
            }
            return View(User);
        }

        public ActionResult Journey(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<Journey> list = journeySer.GetAllJourney();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SearchJourney(String keyword)
        {
            IList<Journey> list = journeySer.GetAllJourney();
            var result = list.Where(u => u.Name.Contains(keyword)).OrderBy(u => u.Name).ToList();
            return PartialView(result);
        }

        public ActionResult DeleteJourney(Guid UserID, Guid JourneyID)
        {
            if (UserID == null)
            {
                TempData["Msg"] = "Delete journey failed";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (JourneyID == null)
            {
                TempData["Msg"] = "Delete journey failed";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                journeySer.DeleteJourney(UserID, JourneyID);
                TempData["Msg"] = "Delete journey successfully";
                return RedirectToAction("Journey");
            }
        }

        public ActionResult DeleteMultipleJourney(Guid UserID, Guid[] JourneyID)
        {
            foreach (var id in JourneyID)
            {
                try
                {
                    journeySer.DeleteJourney(UserID, id);
                    return RedirectToAction("Journey");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            TempData["Msg"] = "Delete multiple journey successfully";
            return RedirectToAction("Journey");
        }

        public ActionResult EditJourney(Guid JourneyID)
        {
            if (JourneyID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Journey JourneyRetrieved = journeySer.GetJourneyDetail(JourneyID);
            return View(JourneyRetrieved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJourney(Journey Journey)
        {
            if (ModelState.IsValid)
            {
                journeySer.UpdateJourneyForAdmin(Journey);
                TempData["Msg"] = "Journey has been updated successfully";
                return RedirectToAction("Journey");
            }
            return View(Journey);
        }
    }
}