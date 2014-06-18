using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Journey
    {
        public String journeyID { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public DateTime takenDate { get; set; }

        public DateTime timestamp { get; set; }

        public int numberOfLikes { get; set; }

    }
}