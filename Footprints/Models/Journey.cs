using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Journey
    {
        public Guid JourneyID { get; set; }

        public Guid UserID { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public DateTimeOffset TakenDate { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public int NumberOfLike { get; set; }
    }
}