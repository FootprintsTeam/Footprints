using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Content
    {
        public Guid ContentID { get; set; }
        public String Name { get; set; }
        public String URL { get; set; }
        public DateTimeOffset TakenDate { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}