using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class User
    {

        public Guid userID { get; set; }

        public String firstName { get; set; }

        public String lastName { get; set; }

        public String email { get; set; }

        public String address { get; set; }

        public String phoneNumber { get; set; }

        public String username { get; set; }

        public String password { get; set; }

        public bool status { get; set; }

        public DateTime joinedDate { get; set; }

        public String profilePicURL { get; set; }

        public String displayName()
        {
            return firstName + " " + lastName;
        }
    }
}