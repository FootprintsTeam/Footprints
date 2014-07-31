using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Services;
using Footprints.Models;

namespace Footprints.Models
{
    public class PersonalViewModel
    {
        public Guid UserID { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Email { get; set; }

        public String Address { get; set; }

        public String PhoneNumber { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }

        public StatusEnum Status { get; set; }

        public DateTimeOffset JoinDate { get; set; }

        public String ProfilePicURL { get; set; }

        public String DisplayName()
        {
            return FirstName + " " + LastName;
        }

        public int NumberOfJourney
        {
            get;
            set;
        }

        public int NumberOfDestination
        {
            get;
            set;
        }

        public int NumberOfFriend
        {
            get;
            set;
        }

        public static PersonalViewModel GetSampleObject()
        {
            var sample = new PersonalViewModel
            {
                UserID = new Guid(),
                UserName = "TrungVT",
                Address = "Yen Bai",
                PhoneNumber = "01668284290",
                Email = "trungk18@hotmail.com"
            };
            return sample;
        }
    }
}