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



        public int getNumberOfLikes(String journeyID)
        {
            var query = Db.Cypher.Match("(journey:Journey)").Where((Journey journey) => journey.journeyID == journeyID).Return(journey => journey.As<Journey>());
            return query.Results.First<Journey>().numberOfLikes;
        }
    }

    public interface IJourneyRepository : IRepository<Journey> { 
    
    }
}
