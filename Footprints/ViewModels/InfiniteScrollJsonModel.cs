using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class InfiniteScrollJsonModel
    {
        public string HTMLString { get; set; }
        public bool NoMoreData { get; set; }
    }
    public class InfiniteScrollPhotoListJsonModel
    {
        public string HTMLString { get; set; }
        public bool NoMoreData { get; set; }
        public IList<string> PhotoList { get; set; }
    }
    public class InfiniteScrollSearchResultJsonModel
    {
        public string HTMLString { get; set; }
        public int BlockNumber { get; set; }
        public bool JourneyNoMoreData { get; set; }
        public bool DestinationNoMoreData { get; set; }
        public bool PlaceNoMoreData { get; set; }
        public bool UserNoMoreData { get; set; }
    }
}