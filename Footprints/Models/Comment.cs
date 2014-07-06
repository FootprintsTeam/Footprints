using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Comment
    {
        public Guid commentID { get; set; }

        public String content{get; set;}

        public Guid destinationID { get; set; }

        public Guid journeyID { get; set; }

        public int numberOfLikes { get; set; }

        public DateTimeOffset timestamp { get; set; }
    }
}