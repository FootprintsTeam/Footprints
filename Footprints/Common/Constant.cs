using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Common
{
    public static class Constant
    {
        public enum ActivitiesType
        {
            JourneyCreate,
            DestinationCreate,
            FriendAdd,
            ImageAdd,
            CommentAdd,
            Like
        }
        public static int DefaultNumberOfLike = 0;
        public static int DefaultNumberOfShare = 0;
        public const String GUID_REGEX = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";
        public const string DEFAULT_AVATAR_FILE_NAME = "default_avatar.001.jpg";
        public const string DEFAULT_COVER_FILE_NAME = "Coverpicture.png";
        public const string DEFAULT_AVATAR_URL = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-588205328470/" + DEFAULT_AVATAR_FILE_NAME;
        public const string DEFAULT_COVER_URL = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-588205328470/" + DEFAULT_COVER_FILE_NAME;
        public const string mapUrl = "http://maps.googleapis.com/maps/api/staticmap";
        public static string UPLOAD_PHOTO_ERROR_MESSAGE = "An error occurred while processing your request";

    }

    public enum Genre
    {
        Male,
        Female
    }

    public enum FunctionResult { success, fail }
}