using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class FriendItemViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public DateTime Time { get; set; }
        public String ProfilePictureUrl { get; set; }
        public static FriendItemViewModel GetSampleObject()
        {
            return new FriendItemViewModel()
            {
                UserID = new Guid(),
                UserName = "Hùng Vi",
                Time = DateTime.Now,
                ProfilePictureUrl = "../assets//images/people/100/9.jpg"
            };
        }
    }
}