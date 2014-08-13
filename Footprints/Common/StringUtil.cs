using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;

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

        public static string ConstructMapImageUrl(this Place place) { 
            //http://maps.googleapis.com/maps/api/staticmap?center=-15.800513,-47.91378&zoom=11&size=200x200&markers=color:blue%7Clabel:S%7C40.702147,-74.015794
            return string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size={3}x{4}&markers=color:{5}%7C{6},{7}",place.Latitude,place.Longitude,12,150,180,ConsoleColor.Blue,place.Latitude,place.Longitude);
        }
    }
}