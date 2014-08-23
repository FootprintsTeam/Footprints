using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class Activity
    {
        public Guid ActivityID { get; set; } 
        public String Type { get; set; }
        public enum StatusEnum { Active, Deleted }
        public StatusEnum Status { get; set; }
        public Guid UserID { get; set; }
        public Guid JourneyID { get; set; }
        public Guid CommentID { get; set; }
        public Guid DestinationID { get; set; }
        public Guid ContentID { get; set; }
        public String PlaceID { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public String SharedContent { get; set; }
        public int NumberOfPhoto { get; set; }        
        public string DestinationName { get; set; }
        public int NumberOfLike { get; set; }
        public int NumberOfShare { get; set; }
        //Friend Object
        public string FriendName { get; set; }
        public string FriendUserID { get; set; }
        public string FriendProfilePicUrl { get; set; }
        // User Object
        public String UserName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String ProfilePicURL { get; set; }
        //Journey Object
        public String Journey_Name { get; set; }
        public String Journey_Description { get; set; }
        public int Journey_NumberOfLike { get; set; }
        public int Journey_NumberOfShare { get; set; }
        //Destination Object
        public String Destination_Name { get; set; }
        public String Destination_Description { get; set; }
        public int Destination_NumberOfLike { get; set; }
        public int Destination_NumberOfShare { get; set; }
        //Place Object
        public String Place_Name { get; set; }
        public String Place_Address { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        //Content Object
        public String ContentURL { get; set; }
        //Comment Object
        public String CommentContent { get; set; }
    }
}