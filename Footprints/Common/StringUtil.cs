using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;
using Footprints.ViewModels;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace Footprints.Common
{
    public static class StringUtil
    {
        public static bool compareGuidToString(Guid objGuid, String strGuid)
        {
            if (objGuid == null || strGuid == null || strGuid.Length == 0)
                return false;
            return (objGuid.ToString().ToLower().Equals(strGuid.ToLower()));
        }

        public static string TruncateLongString(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string ConstructMapImageUrl(this Place place, float width = 150, float height = 150, int zoomLvl = 12) { 
            //http://maps.googleapis.com/maps/api/staticmap?center=-15.800513,-47.91378&zoom=11&size=200x200&markers=color:blue%7Clabel:S%7C40.702147,-74.015794
            return string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size={3}x{4}&markers=color:{5}%7C{6},{7}",place.Latitude,place.Longitude,zoomLvl,width,height,ConsoleColor.Blue,place.Latitude,place.Longitude);
        }

        public static string GetContentIdFromS3Url(string s3FileAbsolutePath, string userId, string albumId)
        {
            return s3FileAbsolutePath.Replace("https://s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/" + System.Configuration.ConfigurationManager.AppSettings["ImageBucketName"] + "/" + userId + "/" + albumId + "/", "").Replace(".jpg", "");
        }

        public static string GetWidgetViewName(this NewsfeedBaseWidgetViewModel widget)
        {
            switch (widget.Type)
            {
                case Constant.ActivityAddNewContent:
                    return "AddPhotoWidget";
                case Constant.ActivityAddNewDestination:
                    return "DestinationWidget";
                case Constant.ActivityAddNewFriend:
                    return "FriendWidget";
                case Constant.ActivityAddnewJourney:
                    return "JourneyWidget";
                case Constant.ActivityComment:
                    return "CommentWidget";
                case Constant.ActivityLikeDestination:
                    System.Diagnostics.Debug.WriteLine(widget.Type);
                    break;
                case Constant.ActivityShareDestination:
                    return "ShareWidget";
                default:
                    System.Diagnostics.Debug.WriteLine(widget.Type + "something");
                    return "";
            }
            return "";
        }

        public static MvcHtmlString ToJson(this HtmlHelper html, object obj)
        {            
            return MvcHtmlString.Create(JsonConvert.SerializeObject(obj));
        }        
    }
}