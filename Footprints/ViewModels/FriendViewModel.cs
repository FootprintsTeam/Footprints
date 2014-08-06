using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;
using System.ComponentModel.DataAnnotations;

namespace Footprints.ViewModels
{
    public class FriendViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public IEnumerable<FriendItemViewModel> FriendList { get; set; }
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