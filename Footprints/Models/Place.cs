using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Footprints.Models
{
    public class Place
    {
        [Required(ErrorMessage = "Please choose a place on the map")]
        public String PlaceID { get; set; }
        [Display(Name = "Place Longitude")]
        [Required(ErrorMessage = "Please choose a place on the map")]
        public Double Longitude { get; set; }
        [Required(ErrorMessage = "Please choose a place on the map")]
        public Double Latitude { get; set; }
        public String Reference { get; set; }
        [Display(Name = "Place Name")]
        [Required(ErrorMessage = "Place Name is required")]
        public String Name { get; set; }
        public String Address { get; set; }
    }
}