using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(IGraphClient client) : base(client) { }

        public int GetNumberOfLike(Guid journeyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").
                Where((Journey journey) => journey.JourneyID == journeyID).
                Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>().NumberOfLike;
        }

        public Journey GetJourneyByID(Guid journeyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").
                Where((Journey journey) => journey.JourneyID == journeyID).
                Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>();
        }

        public bool AddNewJourney(Guid userID, Journey journey)
        {
            //Cypher Query 
            Activity activity = new Activity
            {
                Type = "ADD_NEW_JOURNEY",
                UserID = userID,
                JourneyID = journey.JourneyID,
                Timestamp = DateTime.Today
            };
           
            Db.Cypher.Create("(journey:Journey {j} )").WithParam("j", journey).With("journey").
                    Match("(user:User)").Where((User user) => user.UserID == userID).
                    Create("(user)-[:HAS_JOURNEY]->(journey)").
                    Create("(activity:Activity {a})").WithParam("a", activity).
                    With("user, journey, activity").
                    OptionalMatch("(user)-[f:LATEST_ACTIVITY]->(nextActivity)").
                    Create("(user)-[:LATEST_ACTIVITY]->(activity)").
                    Create("(activity)-[:ACT_ON_JOURNEY]->(journey)").
                    With("f, activity, nextActivity").
                    Where("f IS NOT NULL").Delete("f").
                    Create("(activity)-[:NEXT]->(nextActivity)").ExecuteWithoutResults();
            return true;
        }



        public void UpdateJourney(Journey journey)
        {
            throw new NotImplementedException();
        }

        public void DeleteJourney(Guid journeyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Journey> GetJourneyList()
        {
            throw new NotImplementedException();
        }
    }

    public interface IJourneyRepository : IRepository<Journey>
    {
        bool AddNewJourney(Guid userID, Journey journey);    
        int GetNumberOfLike(Guid journeyID);
        Journey GetJourneyByID(Guid journeyID);
        void UpdateJourney(Journey journey);
        void DeleteJourney(Guid journeyID);
        IEnumerable<Journey> GetJourneyList();
    }
}
