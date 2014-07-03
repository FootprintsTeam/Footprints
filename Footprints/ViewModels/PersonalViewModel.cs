using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class PersonalViewModel
    {
        public Guid UserID { get; set; }

        public string UserName { get; set; }

        public DateTime JoinDate { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }       

        public string UserAvatarURL { get; set; }


        public static PersonalViewModel GetSampleObject()
        {
            var sample = new PersonalViewModel
            {
                UserID = new Guid(),
                UserName = "TrungVT",
                Address = "Yen Bai",
                Phone = "01668284290",
                Email = "trungk18@hotmail.com"
            };
            return sample;
        }
    }
}