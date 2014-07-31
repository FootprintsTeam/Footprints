using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;
using Microsoft.AspNet.Identity;
using Footprints.DAL.Concrete;
namespace Footprints.Common
{
    public class UserMigrator
    {
        IUserRepository userRepository;

        public UserMigrator(IUserRepository userRepository) {
            this.userRepository = userRepository;
        }
        public void migrate()
        {
            //arrange
            ApplicationDbContext context = new ApplicationDbContext();            

            var users = context.Users.ToList();
            foreach (ApplicationUser user in users)
            {
                var u = new User();
                u.UserID = new Guid(user.Id);
                u.Email = user.Email;                
                userRepository.AddNewUser(u);
            };
        }
    }
}