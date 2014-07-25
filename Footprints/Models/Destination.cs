using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Destination
    {
        public Guid DestinationID { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public DateTimeOffset TakenDate { get; set; }
        public int NumberOfLike { get; set; } 
        public int NumberOfShare { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public List<Content> Contents { get; set; }
    }
}