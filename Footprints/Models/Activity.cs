using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Activity
    {
        public String type { get; set; }
        public Guid userID { get; set; }
        public Guid journeyID { get; set; }
        public Guid commentID { get; set; }
        public Guid destinationID { get; set; }
        public DateTimeOffset timestamp { get; set; }
    }
}