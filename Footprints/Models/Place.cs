using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Footprints.Models
{
    public class Place
    {
        public String PlaceID { get; set; }

        [Display(Name = "Place Longitude")]
        [Required(ErrorMessage = "Place Longitude is required")]
        public Double Longitude { get; set; }

        [Display(Name = "Place Latitude")]
        [Required(ErrorMessage = "Place Latitude is required")]
        public Double Latitude { get; set; }

        public String Reference { get; set; }

        [Display(Name = "Place Name")]
        [Required(ErrorMessage = "Place Name is required")]
        public String Name { get; set; }
    }
}