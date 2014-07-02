using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class FriendViewModel
    {
        public Guid UserID { get; set; }
        
        public string UserName { get; set; }

        public DateTime Time { get; set; }

        public string UserAvatarURL { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.Time); }
            private set { }
        }

        public static IEnumerable<FriendViewModel> GetSampleObject()
        {
            var sample = new FriendViewModel
            {
                UserID = new Guid(),
                UserName = "Hung Vi",                
                Time = DateTime.Now                
            };
            List<FriendViewModel> list = new List<FriendViewModel>();
            list.Add(sample);
            return list;
        }
    }
}