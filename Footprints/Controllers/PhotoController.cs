using Footprints.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Footprints.Controllers
{
    public class PhotoController : ApiController
    {
        [HttpPost]
        public async Task<FileInfoList> Post()
        {
            var UserID = new Guid(Guid.NewGuid().ToString("N"));
            var AlbumID = new Guid();

            const string ERROR = "An error occurred while processing your request";
            FileInfoList fileInfoList = new FileInfoList();
            FileInfoItem fileInfoItem = new FileInfoItem();
            fileInfoList.files.Add(fileInfoItem);

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                fileInfoItem.error = ERROR;
                return fileInfoList;
            }


            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                StringBuilder sb = new StringBuilder();
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);
                //Get form data
                Hashtable mapFormData = new Hashtable();
                foreach (var key in provider.FormData.AllKeys)
                {
                    if (mapFormData.ContainsKey(key)) continue;
                    string[] values = provider.FormData.GetValues(key);
                    if (values.Length == 1)
                        mapFormData.Add(key, values[0]);
                    else
                        mapFormData.Add(key, values);
                }
                //Check if request album exists
                if (!mapFormData.ContainsKey("AlbumID") || Guid.TryParse(mapFormData["AlbumID"].ToString(), out AlbumID))
                {
                    fileInfoItem.error = ERROR;
                    return fileInfoList;
                }


                string s3Path = "https://s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/";
                string bucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];

                //Get Uploaded files
                foreach (var file in provider.FileData)
                {
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);
                    sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
                    try
                    {
                        FileStream fileStream = fileInfo.OpenRead();
                        fileInfoItem.size = fileInfo.Length;
                        if (ImageUtil.IsValidImage(fileStream))
                        {

                            string imgFileName = ImageProcessor.UploadPhoto(UserID, AlbumID, fileStream);
                            fileInfoItem.url = s3Path + bucketName + "/" + UserID + "/" + mapFormData["album"].ToString() + "/" + imgFileName;
                            fileInfoItem.deleteUrl = "api/photo/image_id";
                            fileInfoItem.deleteType = "DELETE";
                            //Update uploaded image info into DB
                            //ContentService.AddPhoto(Guid UserID, AlbumID, Guid PhotoID)
                            fileStream.Close();
                        }
                        else
                        {
                            fileInfoItem.error = ERROR;
                            return fileInfoList;
                        }
                    }
                    catch (Exception)
                    {
                        fileInfoItem.error = ERROR;
                        return fileInfoList;
                    }
                }

            }
            catch (Exception)
            {
                fileInfoItem.error = ERROR;
                return fileInfoList;
            }
            return fileInfoList;
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            //Remove image in database
            //ContentService.DeletePhoto(Guid id)
            //Remove image in s3
            ImageProcessor.DeletePhoto(Guid.NewGuid(), Guid.NewGuid(), id.ToString() + ".jpg");
        }
    }

    /// <summary>
    /// Contains list of 
    /// </summary>
    public class FileInfoList
    {
        public List<FileInfoItem> files = new List<FileInfoItem>();
    }

    /// <summary>
    /// Contains information of uploaded file
    /// </summary>
    public class FileInfoItem
    {
        public string name { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get; set; }
        public string error { get; set; }
    }
}
