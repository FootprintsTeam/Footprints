using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class FriendViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public IList<FriendItemViewModel> FriendList = new List<FriendItemViewModel>();
        public static FriendViewModel GetSampleObject()
        {
            var list = new List<FriendItemViewModel>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(FriendItemViewModel.GetSampleObject());
            }
            return new FriendViewModel()
            {
                UserID = Guid.NewGuid(),
                UserName = "Vi Ngọc Hùng",
                FriendList = list
            };
        }
    }
    public class FriendItemViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public DateTime Time { get; set; }
        public String ProfilePicURL { get; set; }
        public static FriendItemViewModel GetSampleObject()
        {
            return new FriendItemViewModel()
            {
                UserID = Guid.NewGuid(),
                UserName = "Hùng Vi",
                Time = DateTime.Now,
                ProfilePicURL = "../assets/images/people/100/22.jpg"
            };
        }
    }

    public class AddFriendViewModel
    {
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public Guid FriendID { get; set; }
    }

    public class UnfriendViewModel
    {
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public Guid FriendID { get; set; }
    }

}