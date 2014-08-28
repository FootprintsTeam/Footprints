using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Footprints.Common;
using Footprints.Models;

namespace Footprints.ViewModels
{
    public class FriendViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public IList<User> FriendList { get; set; }
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