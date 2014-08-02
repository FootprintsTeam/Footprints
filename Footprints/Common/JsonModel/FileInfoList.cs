using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Common.JsonModel
{
    /// <summary>
    /// Contains list of 
    /// </summary>
    public class FileInfoList
    {
        public List<FileInfoItem> files = new List<FileInfoItem>();
    }

    /// <summary>
    /// Contains information of uploaded file
    /// </summary>
    public class FileInfoItem
    {
        public string name { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get; set; }
        public string error { get; set; }
    }
}