﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Common
{
    public class StringUtil
    {
        public static bool compareGuidToString(Guid objGuid, String strGuid)
        {
            if (objGuid == null || strGuid == null || strGuid.Length == 0)
                return false;
            return (objGuid.ToString().ToLower().Equals(strGuid.ToLower()));
        }
    }
}