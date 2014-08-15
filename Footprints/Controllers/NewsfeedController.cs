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
using System.IO;
namespace Footprints.Controllers
{
    [Authorize]
    public class NewsfeedController : Controller
    {
        IUserService userService;
        public NewsfeedController(IUserService userService) {
            this.userService = userService;
        }
        //
        // GET: /Newsfeed/Newsfeed/
        public ActionResult Index()
        {
            return View();
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext
                (ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        //public ActionResult TemplateIndex() {
        //    return View();
        //}

        [ChildActionOnly]
        [ActionName("PersonalWidget1")]
        public ActionResult PersonalWidget(object model)
        {            
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult InfiniteScroll(int BlockNumber)
        {
            ////////////////// THis line of code only for demo. Needs to be removed ////
            //System.Threading.Thread.Sleep(3000);
            ////////////////////////////////////////////////////////////////////////////
            //int BlockSize = 5;
            //var books = DataManager.GetBooks(BlockNumber, BlockSize);
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();
            //jsonModel.NoMoreData = books.Count < BlockSize;
            jsonModel.HTMLString = RenderPartialViewToString("PersonalWidgetViewModel", null);
            return Json(jsonModel);
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult MainNavBar() {
            var userModel = userService.RetrieveUser(new Guid(User.Identity.GetUserId()));
            return PartialView("MainNavBar", userModel);
        }

        [ChildActionOnly]
        public ActionResult AddPhotoWidget() {
            var sample = AddPhotoWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult CommentWidget() {
            var sample = CommentWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult ShareWidget() {
            var sample = ShareWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult PersonalWidget() {
            var sample = PersonalWidgetViewModel.GetSampleObject();
            return PartialView(sample); 
        }

        [ChildActionOnly]
        public ActionResult DestinationWidget()
        {
            var sample = DestinationWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }

        [ChildActionOnly]
        public ActionResult JourneyWidget() {
            var sample = JourneyWidgetViewModel.GetSampleObject();
            return PartialView(sample);
        }
	}
}