using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Footprints.Models;
using Footprints.ViewModels;
using Footprints.Services;
using AutoMapper;

namespace Footprints.Controllers
{
    public class PersonalController : Controller
    {
        IUserService userService;
        IJourneyService journeyService;
        IDestinationService destinationService;

        public PersonalController(IUserService userService, IJourneyService journeyService, IDestinationService destinationService) {
            this.userService = userService;
            this.journeyService = journeyService;
            this.destinationService = destinationService;
        }
        //
        // GET: /Personal/Personal/
        public ActionResult Index()
        {
            var model = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var viewModel = Mapper.Map<User, PersonalViewModel>(model);
            
            //var model = PersonalViewModel.GetSampleObject();            
            return View(viewModel);            
        }
        public ActionResult About()
        {
            var model = PersonalAboutViewModel.GetSampleObject();
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(PersonalAboutViewModel model) {
            return View();
        }

        [HttpGet]
        public ActionResult Update() {
            var model = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            var viewModel = Mapper.Map<User, PersonalAboutViewModel>(model);
            return View(viewModel);
        }

        public ActionResult AddCoverPhoto() {
            var photoContent = TempData["FileInfoList"];
            var userID = new Guid(TempData["MasterID"].ToString());
            var photoInfo = (Content)TempData["MediaContent"];
            
            //add photo
            userService.UpdateCoverPhotoURL(userID, photoInfo.URL);

            //delete temporary data
            TempData.Remove("FileInfoList");
            TempData.Remove("MasterID");
            TempData.Remove("MediaContent");
            return Json(photoContent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAvatarPhoto() {
            var photoContent = TempData["FileInfoList"];
            var userID = new Guid(TempData["MasterID"].ToString());
            var photoInfo = (Content)TempData["MediaContent"];

            //add photo
            userService.UpdateProfilePicURL(userID, photoInfo.URL);

            //delete temporary data
            TempData.Remove("FileInfoList");
            TempData.Remove("MasterID");
            TempData.Remove("MediaContent");
            return Json(photoContent, JsonRequestBehavior.AllowGet);
        }
	}
}