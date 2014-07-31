using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footprints.Models;
using Footprints.DAL.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;

namespace Footprints.Tests.DITest
{
    public class BaseTestClass
    {        
        public static GraphClient client;

        //List Repository
        public static UserRepository userRep;
        public static JourneyRepository journeyRep;

        //List model
        public static IList<User> users;
        public static IList<Journey> journeys;

        public BaseTestClass() {
            client = new GraphClient(new Uri("http://54.179.157.145:7474/db/data"));
            client.Connect();
            SetupUser();
            SetupJourney();
        }

        void SetupUser(){
            users = new List<User>();
            users.Add(new User
            {
                UserID = new Guid(Guid.NewGuid().ToString("N")),
                FirstName = "Nhan",
                LastName = "Trinh"
            });
            userRep = new UserRepository(client);
        }

        void SetupJourney() {
            journeys = new List<Journey>();
            journeys.Add(new Journey
            {
                JourneyID = new Guid(Guid.NewGuid().ToString("N")),
                Description = "Journey Description",
                Name = "Nhan's Journey",
                NumberOfLike = 2,
                TakenDate = DateTimeOffset.Now,
                Timestamp = DateTimeOffset.Now,                
                UserID = users.First().UserID
            });

            journeyRep = new JourneyRepository(client);
        }
    }
}
