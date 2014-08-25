using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Footprints.Models
{
    public class User
    {
        [HiddenInput(DisplayValue = false)]
        public Guid UserID { get; set; }
        public String About { get; set; }      
        [Display(Name="First name")]
        public String FirstName { get; set; }
        [Display(Name = "Last name")]
        public String LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public String Email { get; set; }       
        public String Address { get; set; }
        public String PhoneNumber { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Display(Name="Birthday")]
        public String DateOfBirth { get; set; }
        public String UserName { get; set; }           
        public StatusEnum Status { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        [Display(Name="Avatar")]
        public String ProfilePicURL { get; set; }
        public String CoverPhotoURL { get; set; }
        public String DisplayName()
        {
            return FirstName + " " + LastName;             
        }
    }
    public enum StatusEnum { Banned, Active, Inactive, Admin }
}