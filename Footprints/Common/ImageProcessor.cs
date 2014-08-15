using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using Amazon.S3;
using Amazon.S3.Model;
using Footprints.Common.JsonModel;

namespace Footprints.Common
{
    public class ImageProcessor
    {
        public static void UploadPhotoWithThumb(String UserID, String AlbumID, String FileName, Stream imageStream)
        {
            //Upload image to s3
            String uniqueFileName = FileName.ToString() + ".jpg";
            String photoPath = UserID + "/" + AlbumID + "/" + uniqueFileName;
            String photoThumbPath = UserID + "/" + AlbumID + "/thumbnails/" + uniqueFileName;
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

        public static void DeletePhoto(String UserID, String AlbumID, String ContentID)
        {
            try
            {
                using (IAmazonS3 s3Client = Amazon.AWSClientFactory.CreateAmazonS3Client(Amazon.RegionEndpoint.APSoutheast1))
                {
                    DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest();
                    deleteObjectsRequest.BucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
                    deleteObjectsRequest.AddKey(UserID + "/" + AlbumID + "/" + ContentID + ".jpg");
                    DeleteObjectsResponse deleteObjectResponse = s3Client.DeleteObjects(deleteObjectsRequest);
                }
            }
            catch { }
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

        public static FileInfoList UploadPhoto(String UserID, String AlbumID, String ContentID, Stream imageStream, String deleteUrl)
        {
            FileInfoList fileInfoList = new FileInfoList();
            FileInfoItem fileInfoItem = new FileInfoItem();
            fileInfoList.files.Add(fileInfoItem);
            String s3Path = "https://s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/";
            String bucketName = System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"];
            try
            {
                if (ImageUtil.IsValidImage(imageStream))
                {
                    fileInfoItem.size = imageStream.Length;
                    UploadPhotoWithThumb(UserID, AlbumID, ContentID, imageStream);
                    fileInfoItem.url = s3Path + bucketName + "/" + UserID + "/" + AlbumID + "/" + ContentID + ".jpg";
                    fileInfoItem.thumbnailUrl = s3Path + bucketName + "/" + UserID + "/" + AlbumID + "/thumbnails/" + ContentID + ".jpg";
                    fileInfoItem.deleteUrl = deleteUrl;
                    fileInfoItem.deleteType = "DELETE";
                }
                else
                {
                    fileInfoItem.error = Constant.UPLOAD_PHOTO_ERROR_MESSAGE;
                }
            }
            catch (Exception)
            {
                fileInfoItem.error = Constant.UPLOAD_PHOTO_ERROR_MESSAGE;
            }
            return fileInfoList;
        }
    }
}