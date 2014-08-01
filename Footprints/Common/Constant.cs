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
    }
}