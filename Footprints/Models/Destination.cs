using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Destination
    {
        public Guid destinationID { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public DateTimeOffset takenDate { get; set; }

        public int numberOfLikes { get; set; }

        public DateTimeOffset timestamp { get; set; }

        public List<Content> contents { get; set; }
    }
}