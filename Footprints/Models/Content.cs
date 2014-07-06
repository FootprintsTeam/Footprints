using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Content
    {
        public Guid contentID { get; set; }

        public String name { get; set; }

        public String URL { get; set; }

        public DateTimeOffset takenDate { get; set; }

        public DateTimeOffset timestamp { get; set; }

    }
}