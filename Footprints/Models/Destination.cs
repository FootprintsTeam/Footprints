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
        public String Name { get; set; }          
        public int OrderNumber { get; set; }        
        public String Description { get; set; }        
        public DateTimeOffset TakenDate { get; set; }                
        public int NumberOfLike { get; set; }                
        public int NumberOfShare { get; set; }        
        public DateTimeOffset Timestamp { get; set; }
        public Place Place { get; set; }
        public List<Content> Contents { get; set; }
        public List<Comment> Comments { get; set; }
    }
}