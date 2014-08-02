using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using Amazon.S3;
using Amazon.S3.Model;

namespace Footprints.Common
{
    public class ImageProcessor
    {
        public static void UploadPhotoWithThumb(Guid UserID, Guid AlbumID, Guid FileName, Stream imageStream)
        {
            //Upload image to s3
            String uniqueFileName = FileName.ToString() + ".jpg";
            String photoPath = UserID.ToString() + "/" + AlbumID.ToString() + "/" + uniqueFileName;
            String photoThumbPath = UserID.ToString() + "/" + AlbumID.ToString() + "/thumbnails/" + uniqueFileName;
            try
            {
                //Generate thumbnails image
                System.Drawing.Image uploadImage = System.Drawing.Image.FromStream(imageStream);
                System.Drawing.Image resizedImage = ImageUtil.ResizeThumbnail(uploadImage);
                MemoryStream ms = new MemoryStream();
                resizedImage.Save(ms, ImageFormat.Jpeg);
                //Put thumbnail photo to album
                UploadPhoto(photoThumbPath, ms);
                //Put original image file to album
                UploadPhoto(photoPath, imageStream);
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public static void DeletePhoto(Guid UserID, Guid AlbumID, string name)
        {
            using (IAmazonS3 s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client(Amazon.RegionEndpoint.APSoutheast1))
            {
                DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest();
                deleteObjectsRequest.BucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
                deleteObjectsRequest.AddKey(UserID.ToString() + "/" + AlbumID.ToString() + "/" + name);
                DeleteObjectsResponse deleteObjectResponse = s3Client.DeleteObjects(deleteObjectsRequest);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">absolute path to store image in the bucket, for example, userid/albumid/GUID.jpg</param>
        /// <param name="imageStream">stream of the uploaded photo</param>
        /// <param name="client">AWS S3 client</param>
        public static void UploadPhoto(string key, Stream imageStream)
        {
            try
            {
                using (IAmazonS3 s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client(Amazon.RegionEndpoint.APSoutheast1))
                {
                    PutObjectRequest putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
                    putObjectRequest.CannedACL = S3CannedACL.PublicRead;
                    putObjectRequest.InputStream = imageStream;
                    putObjectRequest.Key = key;
                    PutObjectResponse response = s3Client.PutObject(putObjectRequest);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Upload image stream to Amazon s3
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="albumid"></param>
        /// <param name="imageStream"></param>
        /// <param name="client"></param>
        public static string UploadPhoto(Guid UserID, Guid AlbumID, Stream imageStream)
        {
            string photoPath = UserID.ToString() + "/" + AlbumID.ToString() + "/" + new Guid(Guid.NewGuid().ToString("N")) + ".jpg";
            UploadPhoto(photoPath, imageStream);
            return photoPath;
        }
    }
}