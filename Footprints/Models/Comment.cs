using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Comment
    {
        public String commentID { get; set; }

        public String content{get; set;}

        public String destinationID { get; set; }

        public String journeyID { get; set; }

        public int numberOfLikes { get; set; }

        public DateTime timestamp { get; set; }
    }
}