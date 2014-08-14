using Footprints.Common;
using Footprints.Common.JsonModel;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Footprints.Services;
using Footprints.Models;

namespace Footprints.Controllers
{
    public class MediaController : Controller
    {
        IDestinationService destinationService;
        public MediaController(IDestinationService destinationService)
        {
            this.destinationService = destinationService;
        }
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
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto(ImageViewModel model)
        {
            String ReturnUrl = model.ReturnUrl;
            Guid MasterID = model.MasterID;
            var regexGuid = new Regex(Common.Constant.GUID_REGEX);
            //Authorize
            var UserID = new Guid(User.Identity.GetUserId());
            FileInfoList fileInfoList = new FileInfoList();
            FileInfoItem fileInfoItem = new FileInfoItem();
            fileInfoList.files.Add(fileInfoItem);
            const string ERROR_MESSAGE = "An error occurred while processing your request";

            var AlbumID = MasterID;


            var ContentID = Guid.NewGuid();
            String s3Path = "https://s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/";
            String bucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
            try
            {
                if (ImageUtil.IsValidImage(Request.Files.Get(0).InputStream))
                {
                    fileInfoItem.size = Request.Files.Get(0).InputStream.Length;
                    ImageProcessor.UploadPhotoWithThumb(UserID, AlbumID, ContentID, Request.Files.Get(0).InputStream);
                    fileInfoItem.url = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/" + ContentID.ToString() + ".jpg";
                    fileInfoItem.thumbnailUrl = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/thumbnails/" + ContentID.ToString() + ".jpg";
                    fileInfoItem.deleteUrl = this.Url.Action("DeletePhoto", "Media", new { id = ContentID }, this.Request.Url.Scheme);
                    fileInfoItem.deleteType = "DELETE";
                    var mediaContent = new Content
                    {
                        ContentID = ContentID,
                        TakenDate = DateTimeOffset.Now,
                        Timestamp = DateTimeOffset.Now,
                        URL = fileInfoItem.url
                    };
                    TempData["MediaContent"] = mediaContent;
                }
                else
                {
                    fileInfoItem.error = ERROR_MESSAGE;
                }
            }
            catch (Exception e)
            {
                fileInfoItem.error = ERROR_MESSAGE;
                fileInfoItem.error = "ERROR";
                return Json(fileInfoList, JsonRequestBehavior.AllowGet);
            }
            TempData["FileInfoList"] = fileInfoList;
            TempData["MasterID"] = Request.Form["MasterID"];
            return Redirect(Request.Form["ReturnUrl"]);
        }

        public ActionResult CreateAlbum()
        {
            return View();
        }

        public ActionResult DeletePhoto(Guid id)
        {
            System.Diagnostics.Debug.WriteLine("PhotoID - id = [" + id.ToString() + "]");
            return View();
        }
    }
}
