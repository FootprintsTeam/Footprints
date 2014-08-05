﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public String About { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String PhoneNumber { get; set; }
        public enum GenreEnum { Male, Female, Others}
        public GenreEnum Genre { get; set; }
        public String DateOfBirth { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }        
        public StatusEnum Status { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        public String ProfilePicURL { get; set; }
        public String CoverPhotoUrl { get; set; }
        public String DisplayName()
        {
            return FirstName + " " + LastName;
        }
    }

    public enum StatusEnum { Baned, Active, Inactive, Admin }
}