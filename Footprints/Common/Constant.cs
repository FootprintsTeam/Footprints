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
    }
}