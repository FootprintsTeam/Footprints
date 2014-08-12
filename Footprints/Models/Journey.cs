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
        [Required(ErrorMessage="Name of Journey is required")]
        public String Name { get; set; }
        public String Description { get; set; }
        [Display(Name="Taken Date")]
        [Required(ErrorMessage = "Taken Date is required")]
        public DateTimeOffset TakenDate { get; set; }
        [Display(Name = "Time Stamp")]
        [Required(ErrorMessage = "Time Stamp is required")]
        public DateTimeOffset Timestamp { get; set; }
        [Display(Name = "Number Of Like")]
        [Range(0, int.MaxValue, ErrorMessage="Number of like must be a positive number")]
        public int NumberOfLike { get; set; }
        [Display(Name = "Number Of Share")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of share must be a positive number")]
        public int NumberOfShare { get; set; }
        public List<Destination> Destinations { get; set; }
    }
}