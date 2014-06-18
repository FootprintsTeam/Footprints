using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Destination
    {
        public String destinationID { get; set; }

        public String name { get; set; }

        public String desription { get; set; }

        public float longitude { get; set; }

        public float latitude { get; set; }

        public DateTime takenDate { get; set; }

        public int numberOfLikes { get; set; }

        public DateTime timestamp { get; set; }

        public List<Content> contents { get; set; }
    }
}