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

        public int getNumberOfLikes(Guid journeyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").Where((Journey journey) => journey.journeyID == journeyID).Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>().numberOfLikes;
        }

        public Journey getJourneyByID(Guid journeyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").Where((Journey journey) => journey.journeyID == journeyID).Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>();
        }

        public bool addNewJourney(Guid userID, Journey journey)
        {
            //Cypher Query
            //CREATE (journey:Journey {journeyID : '2', name : 'Hanoi', description : 'Hanoi', takenDate : '03/07/2014', timestamp : '03/07/2014', numberOfLikes : '0'})
            //WITH journey
            //MATCH (user:User)
            //WHERE (user.userID = '1')
            //CREATE (user)-[:HAS_JOURNEY]->(journey)
            //CREATE (activity:Activity { type : 'CREATE_NEW_JOURNEY', journeyID : '2', timestamp : '03/07/2014'})
            //WITH user, journey, activity
            //OPTIONAL MATCH (user)-[f:FIRST]->(nextActivity)
            //CREATE (user)-[:FIRST]->(activity)
            //CREATE (activity)-[:ACT_ON_JOURNEY]->(journey)
            //WITH f, activity, nextActivity
            //WHERE f IS NOT NULL
            //DELETE f
            //CREATE (activity)-[:NEXT]->(nextActivity)
            Activity activity = new Activity {
                type = "ADD_NEW_JOURNEY",
                userID = userID,
                journeyID = journey.journeyID,
                timeStamp = DateTime.Today
            };
            Db.Cypher.Create("(journey:Journey {journey} )").WithParams(new { journey }).With("journey").
                    Match("user:User").Where((User user) => user.userID == userID).
                    Create("(user)-[:HAS_JOURNEY]->(journey)").
                    Create("(activity:Activity {activity})").WithParams(new { activity }).
                    With("user, journey, activity").
                    OptionalMatch("(user)-[f:FIRST]->(nextActivity)").
                    Create("(user)-[:FIRST]->(activity)").
                    Create("(activity)-[:ACT_ON_JOURNEY]->(journey)").
                    With("f, activity, nextActivity").
                    Where("f IS NOT NULL").Delete("f").
                    Create("(activity)-[:NEXT]->(nextActivity)").ExecuteWithoutResults();
            return true;
        }

    }

    public interface IJourneyRepository : IRepository<Journey> {
        public int getNumberOfLikes(Guid journeyID);
        public Journey getJourneyByID(Guid journeyID);
    }
}
