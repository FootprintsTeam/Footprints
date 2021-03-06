﻿using System;
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
using Footprints.Common;
using Footprints.ViewModels;

namespace Footprints.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public const int pageSize = 10;
        public IUserService userSer;
        public IJourneyService journeySer;
        public IDestinationService destinationSer;
        public ISearch FullSearch;
        
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public AdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public AdminController(IUserService userSer, IJourneyService journeySer, IDestinationService destinationSer, ISearch FullSearch)
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            this.userSer = userSer;
            this.journeySer = journeySer;
            this.destinationSer = destinationSer;
            this.FullSearch = FullSearch;
        }
        //
        // GET: /Admin/       

        public ActionResult Index(DashboardViewModel dashView)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            String format = "{yyyy-MM-dd}";

            long totalUser = userSer.GetTotalUser();
            long totalJourney = journeySer.GetNumberOfJourney();
            long totalDestination = destinationSer.GetNumberOfDestination();
            long todayuser = userSer.GetNumberOfRegisterUserBetweenDays(today.ToString(format), tomorrow.ToString(format));
            long todayJourney = journeySer.GetNumberOfCreatedJourneyBetweenDays(today.ToString(format), tomorrow.ToString(format));
            long todayDestination = destinationSer.GetNumberOfCreatedDestinationBetweenDays(today.ToString(format), tomorrow.ToString(format));
            //asssign values to view models
            dashView.TotalDestination = totalDestination;
            dashView.TotalJourney = totalJourney;
            dashView.TotalUser = totalUser;
            dashView.TodayRegisterUser = todayuser;
            dashView.TodayNewJourney = todayJourney;
            dashView.TodayNewDestination = todayDestination;

            return View(dashView);
        }

        public ActionResult Destination(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<Destination> list = destinationSer.GetAllDestination();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SearchDestination(String keyword)
        {
            if (!keyword.Equals(""))
            {
                IList<Destination> list = FullSearch.SearchDestination(keyword, 1000);
                if (list != null)
                {
                    return PartialView(list);
                }
                else
                {
                    return PartialView("NoResult");
                }
            }
            else
            {
                IList<Destination> listAll = destinationSer.GetAllDestination();
                return PartialView(listAll);
            }
        }

        public ActionResult DeleteDestination(Guid DestinationID)
        {
            if (DestinationID == null)
            {
                TempData["Msg"] = "Delete destination failed";
            }
            else
            {
                destinationSer.DeleteDestinationForAdmin(DestinationID);
                TempData["Msg"] = "Delete destination successfully";
            }
            return RedirectToAction("Destination");
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
                return RedirectToAction("Destination");
            }
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
            if (!keyword.Equals(""))
            {
                IList<User> list = FullSearch.SearchUser(keyword, 1000);
                if (list != null)
                {
                    return PartialView(list);
                }
                else
                {
                    return PartialView("NoResult");
                }
            }
            else
            {
                IList<User> listAll = userSer.GetUser();
                return PartialView(listAll);
            }
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
            var rolesRemove = UserManager.RemoveFromRoles(UserID.ToString(), UserRetrieved.Status.ToString());
            if (rolesRemove.Succeeded)
            {
                return View(UserRetrieved);
            }
            else
            {
                return View(UserRetrieved);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User UpdatedUser)
        {
            if (ModelState.IsValid)
            {                
                IdentityResult user = UserManager.SetEmail(UpdatedUser.UserID.ToString(), UpdatedUser.Email.ToString());
                if (user.Succeeded)
                {
                    if (UpdatedUser.Status.Equals(Footprints.Models.StatusEnum.Active))
                    {
                        if (UserManager.IsEmailConfirmed(UpdatedUser.UserID.ToString())) {
                            IdentityResult roleResult = UserManager.AddToRole(UpdatedUser.UserID.ToString(), "Active");
                            if (roleResult.Succeeded)
                            {
                                UpdatedUser.Status = StatusEnum.Active;
                                userSer.UpdateUser(UpdatedUser);
                                TempData["Msg"] = "User has been updated successfully";
                                return RedirectToAction("UserList");
                            }
                            else
                            {
                                TempData["Msg"] = "Update user information failed !";
                                return RedirectToAction("UserList");
                            }
                        }
                        else
                        {                            
                            using (var context = new ApplicationDbContext())
                            {
                                var AppUser = context.Users.Single(x => x.Id == UpdatedUser.UserID.ToString());                                
                                AppUser.EmailConfirmed = true;                                
                                context.SaveChanges();                                
                            }
                            IdentityResult roleResult = UserManager.AddToRole(UpdatedUser.UserID.ToString(), "Active");
                            if (roleResult.Succeeded)
                            {
                                UpdatedUser.Status = StatusEnum.Active;
                                userSer.UpdateUser(UpdatedUser);
                                TempData["Msg"] = "User has been updated successfully";
                                return RedirectToAction("UserList");
                            }
                            else
                            {
                                TempData["Msg"] = "Update user information failed !";
                                return RedirectToAction("UserList");
                            }
                        }
                    }
                    else if (UpdatedUser.Status.Equals(Footprints.Models.StatusEnum.Admin))
                    {
                        IdentityResult roleResult = UserManager.AddToRole(UpdatedUser.UserID.ToString(), "Admin");
                        if (roleResult.Succeeded)
                        {
                            UpdatedUser.Status = StatusEnum.Admin;
                            userSer.UpdateUser(UpdatedUser);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                        else
                        {
                            TempData["Msg"] = "Update user information failed !";
                            return RedirectToAction("UserList");
                        }
                    }
                    else if (UpdatedUser.Status.Equals(Footprints.Models.StatusEnum.Banned))
                    {
                        IdentityResult roleResult = UserManager.AddToRole(UpdatedUser.UserID.ToString(), "Banned");
                        if (roleResult.Succeeded)
                        {
                            UpdatedUser.Status = StatusEnum.Banned;
                            userSer.UpdateUser(UpdatedUser);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                        else
                        {
                            TempData["Msg"] = "Update user information failed !";
                            return RedirectToAction("UserList");
                        }
                    }
                    else if (UpdatedUser.Status.Equals(Footprints.Models.StatusEnum.Unconfirmed))
                    {

                        IdentityResult roleResult = UserManager.AddToRole(UpdatedUser.UserID.ToString(), "Unconfirmed");
                        if (roleResult.Succeeded)
                        {
                            UpdatedUser.Status = StatusEnum.Unconfirmed;
                            userSer.UpdateUser(UpdatedUser);
                            TempData["Msg"] = "User has been updated successfully";
                            return RedirectToAction("UserList");
                        }
                        else
                        {
                            TempData["Msg"] = "Update user information failed !";
                            return RedirectToAction("UserList");
                        }
                    }
                    else
                    {
                        TempData["Msg"] = "Update user information failed !";
                        return RedirectToAction("UserList");
                    }
                }
                else
                {
                    TempData["Msg"] = "Update user information failed !";
                    return RedirectToAction("UserList");
                }
            }
            return RedirectToAction("UserList");

        }

        public ActionResult Journey(int? page)
        {
            int pageNumber = (page ?? 1);
            IList<Journey> list = journeySer.GetAllJourney();
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SearchJourney(String keyword)
        {
            if (!keyword.Equals(""))
            {
                IList<Journey> list = FullSearch.SearchJourney(keyword, 1000);
                if (list != null)
                {
                    return PartialView(list);
                }
                else
                {
                    return PartialView("NoResult");
                }
            }
            else
            {
                IList<Journey> listAll = journeySer.GetAllJourney();
                return PartialView(listAll);
            }
        }

        public ActionResult DeleteJourney(Guid JourneyID)
        {
            if (JourneyID == null)
            {
                TempData["Msg"] = "Delete journey failed";
            }
            else
            {
                journeySer.DeleteJourneyForAdmin(JourneyID);
                TempData["Msg"] = "Delete journey successfully";
            }
            return RedirectToAction("Journey");
        }

        public ActionResult DeleteMultipleJourney(Guid[] Id)
        {
            foreach (var jid in Id)
            {
                try
                {
                    //journeySer.DeleteJourney(UserID, jid);
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
            else
            {
                TempData["Msg"] = "Update journey failed !";
                return RedirectToAction("Journey");
            }
        }
    }
}