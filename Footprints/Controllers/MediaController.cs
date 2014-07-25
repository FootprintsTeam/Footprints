using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Controllers
{
    public class MediaController : Controller
    {

        //
        // GET: /Media/
        public ActionResult Index()
        {
            var model = MediaViewModel.GetSampleObject();
            return View(model);
        }

        public ActionResult AllPhotos()
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Albums()
        {
            var model = AlbumsViewModel.GetSampleObject();
            return View(model);
        }

        public ActionResult AlbumDetails(String albumid)
        {
            return View(AlbumDetailsViewModel.GetSampleObject());
        }

        public ActionResult AddPhotos()
        {
            return View();
        }

        public ActionResult CreateAlbum()
        {
            return View();
        }
    }
}
