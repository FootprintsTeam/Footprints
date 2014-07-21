using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Report
    {
        public Guid reporterID { get; set; }
        public Guid reporteeID { get; set; }
        public Guid postID { get; set; }
        public String reason { get; set; }
        
        // Is this Report seen by Admin
        public bool isProcessed { get; set; }
    }
}