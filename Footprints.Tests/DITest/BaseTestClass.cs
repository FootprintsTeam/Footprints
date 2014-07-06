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
            client = new GraphClient(new Uri("http://localhost:7474/db/data"));
            client.Connect();
            SetupUser();
            SetupJourney();
        }

        void SetupUser(){
            users = new List<User>();
            users.Add(new User
            {
                userID = new Guid(),
                firstName = "Nhan",
                lastName = "Trinh"
            });
            userRep = new UserRepository(client);
        }

        void SetupJourney() {
            journeys = new List<Journey>();
            journeys.Add(new Journey
            {
                journeyID = new Guid(),
                description = "Journey Description",
                name = "Nhan's Journey",
                numberOfLikes = 2,
                takenDate = DateTimeOffset.Now,
                timestamp = DateTimeOffset.Now,                
                userID = users.First().userID
            });

            journeyRep = new JourneyRepository(client);
        }
    }
}
