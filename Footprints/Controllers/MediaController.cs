using Footprints.Common;
using Footprints.Common.JsonModel;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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


        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto()
        {
            FileInfoList fileInfoList = new FileInfoList();
            FileInfoItem fileInfoItem = new FileInfoItem();
            fileInfoList.files.Add(fileInfoItem);

            String AlbumID = Request.Form["AlbumID"];
            var UserID = new Guid(Guid.NewGuid().ToString("N"));
            var ImageID = new Guid(Guid.NewGuid().ToString("N"));
            var regexGuid = new Regex(Common.Constant.GUID_REGEX);
            if (AlbumID == null || !regexGuid.IsMatch(AlbumID) || Request.Files.Count != 1)
            {
                return null;
            }
            string s3Path = "https://s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/";
            string bucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];

            const string ERROR_MESSAGE = "An error occurred while processing your request";

            try
            {
                if (ImageUtil.IsValidImage(Request.Files.Get(0).InputStream))
                {
                    fileInfoItem.size = Request.Files.Get(0).InputStream.Length;
                    ImageProcessor.UploadPhotoWithThumb(UserID, new Guid(AlbumID), ImageID, Request.Files.Get(0).InputStream);
                    fileInfoItem.url = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/" + ImageID.ToString() + ".jpg";
                    fileInfoItem.thumbnailUrl = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/thumbnails/" + ImageID.ToString() + ".jpg";
                    fileInfoItem.deleteUrl = this.Url.Action("DeletePhoto", "Media", new { id = ImageID }, this.Request.Url.Scheme);
                    fileInfoItem.deleteType = "DELETE";
                }
                else
                {
                    fileInfoItem.error = ERROR_MESSAGE;
                }
                // nhan added
                TempData["FileInfoList"] = fileInfoList;
                TempData["MasterID"] = Request.Form["MasterID"];
            }
            catch (Exception e)
            {
                fileInfoItem.error = ERROR_MESSAGE;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

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
