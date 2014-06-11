using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using Amazon.S3;
using Amazon.S3.Model;

namespace Footprints.Core.ImageProcessors
{
    public class ImageUploader
    {
        public static bool UploadPhoto(string userid, string albumid, FileUpload fileUpload, IAmazonS3 s3Client)
        {
            //Check file name, file content
            Debug.WriteLine("File size = {0}", fileUpload.FileBytes.Length);
            Debug.WriteLine(System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"]);
            Debug.WriteLine("Temp file name2: " + Guid.NewGuid().ToString());
            Debug.WriteLine("File extension: " + fileUpload.PostedFile.ContentType);
            //Upload image to s3
            string uniqueFileName = Guid.NewGuid().ToString().Replace('.', '-');
            string photoPath = userid + "/" + albumid + "/" + uniqueFileName + ".jpg";
            string photoThumbPath = userid + "/" + albumid + "/thumbnails/" + uniqueFileName + ".jpg";
            try
            {
                //Generate thumbnails image
                System.Drawing.Image uploadImage = System.Drawing.Image.FromStream(fileUpload.PostedFile.InputStream);
                System.Drawing.Image resizedImage = ImageUtil.ResizeThumbnail(uploadImage);
                MemoryStream ms = new MemoryStream();
                resizedImage.Save(ms, ImageFormat.Jpeg);
                //Put thumbnail photo to album
                UploadPhoto(photoThumbPath, ms, s3Client);
                //Put original image file to album
                UploadPhoto(photoPath, fileUpload.PostedFile.InputStream, s3Client);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">absolute path to store image in the bucket, for example, userid/albumid/GUID.jpg</param>
        /// <param name="imageStream">stream of the uploaded photo</param>
        /// <param name="client">AWS S3 client</param>
        public static void UploadPhoto(string key, Stream imageStream, IAmazonS3 client)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest();
            putObjectRequest.BucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
            putObjectRequest.CannedACL = S3CannedACL.PublicRead;
            putObjectRequest.InputStream = imageStream;
            putObjectRequest.Key = key;
            PutObjectResponse response = client.PutObject(putObjectRequest);
        }

        /// <summary>
        /// Upload photo
        /// </summary>
        /// <param name="userid">user ID</param>
        /// <param name="albumid">album ID</param>
        /// <param name="fileUpload">Image file uploaded from client</param>
        /// <returns>true: uploading successfully, false: otherwise</returns>
        public static bool UploadPhoto(string userid, string albumid, FileUpload fileUpload)
        {
            if (IsValidFileUpload(fileUpload))
            {
                using (IAmazonS3 s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client(Amazon.RegionEndpoint.APSoutheast1))
                {
                    return UploadPhoto(userid, albumid, fileUpload, s3Client);
                }
            }
            return false;
        }

        /// <summary>
        /// Validate uploaded file
        /// </summary>
        /// <param name="fileUpload">uploaded file</param>
        /// <returns>true: fileUpload is image file</returns>
        public static bool IsValidFileUpload(FileUpload fileUpload)
        {
            try
            {
                using (var bitmap = new System.Drawing.Bitmap(fileUpload.PostedFile.InputStream))
                {
                    if (bitmap.Size.IsEmpty)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}