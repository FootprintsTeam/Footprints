﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Comment
    {
        public Guid CommentID { get; set; }
        [Required]
        public String Content{get; set;}
        public Guid DestinationID { get; set; }
        public Guid JourneyID { get; set; }
        public int NumberOfLike { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public User User { get; set; }
    }
}
