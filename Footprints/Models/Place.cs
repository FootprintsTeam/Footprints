using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Place
    {
        public String PlaceID { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public String Reference { get; set; }        
        public String Name { get; set; }
    }
}