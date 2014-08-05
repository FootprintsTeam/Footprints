using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Services;
using Footprints.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Footprints.ViewModels
{
    public class PersonalViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public Guid UserID { get; set; }

        public String CoverPhotoUrl { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }
        
        public String Email { get; set; }

        public String Address { get; set; }

        public String PhoneNumber { get; set; }

        public String UserName { get; set; }

        public StatusEnum Status { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date in the format dd/mm/yyyy hh:mm")]

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
                Email = "trungk18@hotmail.com",
                NumberOfDestination = 20,
                NumberOfFriend = 100,
                NumberOfJourney = 10,
                JoinDate = DateTime.Today,
                ProfilePicURL = "../assets/images/people/250/22.jpg",
                FirstName = "Trung",
                LastName = "Vo Tuan"
            };
            return sample;
        }
    }
}