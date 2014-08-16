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
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        [Display(Name="Taken Date")]
        [Required]
        public DateTimeOffset TakenDate { get; set; }
        [Display(Name = "Time Stamp")]
        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Display(Name = "Number Of Like")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage="Number of like must be a positive number")]
        public int NumberOfLike { get; set; }

        [Display(Name = "Number Of Share")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of share must be a positive number")]
        public int NumberOfShare { get; set; }
        public List<Destination> Destinations { get; set; }
    }
}