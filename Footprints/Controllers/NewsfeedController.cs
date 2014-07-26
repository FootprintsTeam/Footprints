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
using System.IO;
namespace Footprints.Controllers
{
    public class NewsfeedController : Controller
    {
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
        public ActionResult PersonalWidget(object model)
        {            
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult InfiniteScroll(int BlockNumber)
        {
            ////////////////// THis line of code only for demo. Needs to be removed ////
            System.Threading.Thread.Sleep(3000);
            ////////////////////////////////////////////////////////////////////////////
            //int BlockSize = 5;
            //var books = DataManager.GetBooks(BlockNumber, BlockSize);
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();
            //jsonModel.NoMoreData = books.Count < BlockSize;
            jsonModel.HTMLString = RenderPartialViewToString("PersonalWidget", null);
            return Json(jsonModel);
        } 
	}
}