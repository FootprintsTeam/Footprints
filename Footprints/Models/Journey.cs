using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Footprints.Models
{
    public class Journey
    {
        public Guid JourneyID { get; set; }
        public Guid UserID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        [Display(Name="Taken Date")]
        public DateTimeOffset TakenDate { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTimeOffset Timestamp { get; set; }
        [Display(Name = "Number Of Like")]
        public int NumberOfLike { get; set; }
        [Display(Name = "Number Of Share")]
        public int NumberOfShare { get; set; }
        public List<Destination> Destinations { get; set; }
    }
}