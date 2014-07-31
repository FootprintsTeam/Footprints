using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Activity
    {
        public Guid ActivityID { get; set; } 
        public String Type { get; set; }
        public enum StatusEnum { Deleted}
        public StatusEnum Status { get; set; }
        public Guid UserID { get; set; }
        public Guid JourneyID { get; set; }
        public Guid CommentID { get; set; }
        public Guid DestinationID { get; set; }
        public Guid ContentID { get; set; }
        public String PlaceID { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public String Content { get; set; }
    }
}