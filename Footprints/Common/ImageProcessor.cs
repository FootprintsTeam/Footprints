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

        public static FileInfoList UploadPhoto(Guid UserID, Guid AlbumID, Guid ContentID, Stream imageStream, String deleteUrl)
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
                    ImageProcessor.UploadPhotoWithThumb(UserID, AlbumID, ContentID, imageStream);
                    fileInfoItem.url = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/" + ContentID.ToString() + ".jpg";
                    fileInfoItem.thumbnailUrl = s3Path + bucketName + "/" + UserID.ToString() + "/" + AlbumID.ToString() + "/thumbnails/" + ContentID.ToString() + ".jpg";
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