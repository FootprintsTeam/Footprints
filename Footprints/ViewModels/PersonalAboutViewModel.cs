using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class PersonalAboutViewModel
    {
        public Guid UserID { get; set; }
        public String UserName { get; set; }
        public DateTime JoinDate { get; set; }
        public String Gender { get; set; }
        public String AboutMe { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String ProfilePictureUrl { get; set; }
        public int NumberOfJourney { get; set; }
        public int NumberOfDestinations { get; set; }
        public int NumberOfFriends { get; set; }

        public static PersonalAboutViewModel GetSampleObject()
        {
            return new PersonalAboutViewModel
            {
                UserID = new Guid(),
                UserName = "Hùng VN",
                JoinDate = DateTime.Now,
                Gender = "Male",
                AboutMe = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Voluptatibus, eveniet, dolores, sit omnis facere odio rerum laudantium temporibus nam nulla ratione quam quae sint illo similique quis nesciunt repudiandae quisquam.",
                Address = "Address Address Address Address Address Address Address Address",
                Phone = "(+84) 915345353",
                Email = "thisismyemailaddress@gmail.com",
                ProfilePictureUrl = "assets//images/people/250/22.jpg",
                NumberOfJourney = 10,
                NumberOfDestinations = 100,
                NumberOfFriends = 5000
            };
        }
    }
}