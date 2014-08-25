using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Footprints.Models
{
    public class Destination
    {
        public Guid DestinationID { get; set; }
        public Guid AlbumID { get; set; }
        public Guid UserID { get; set; }
        public Guid JourneyID { get; set; }
        [Required]
        public String Name { get; set; }
        [Display(Name = "Order Number")]
        [Required]
        public int OrderNumber { get; set; }
        [Required]
        public String Description { get; set; }
        [Display(Name = "Taken Date")]
        [Required]
        public DateTimeOffset TakenDate { get; set; }
        [Display(Name = "Number Of Like")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of share must be a positive number")]
        public int NumberOfLike { get; set; }
        [Display(Name = "Number Of Share")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of share must be a positive number")]
        public int NumberOfShare { get; set; }
        [Display(Name = "Time Stamp")]
        [Required]
        public DateTimeOffset Timestamp { get; set; }
        public Place Place { get; set; }
        public List<Content> Contents { get; set; }
        public List<Comment> Comments { get; set; }
    }
}