using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.DAL.Concrete
{
    public class Journeys : RepositoryBase<Journey>, IJourneyRepository
    {
        public Journeys(IGraphClient client) : base(client) { }
    }

    public interface IJourneyRepository : IRepository<Journey> { 
    
    }
}
