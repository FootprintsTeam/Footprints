using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class FriendViewModel
    {
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
                FriendList = list
            };

        }
    }
}