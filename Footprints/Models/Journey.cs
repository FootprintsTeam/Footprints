using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Journey
    {
        public Guid journeyID { get; set; }

        public Guid userID { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public DateTimeOffset takenDate { get; set; }

        public DateTimeOffset timestamp { get; set; }

        public int numberOfLikes { get; set; }
    }
}