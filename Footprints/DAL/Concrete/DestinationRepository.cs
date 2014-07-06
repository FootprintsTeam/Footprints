using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
using Footprints.DAL.Concrete;

namespace Footprints.DAL.Concrete
{
    public class DestinationRepository : RepositoryBase<DestinationRepository>, IDestinationRepository
    {
        public DestinationRepository(IGraphClient client) : base(client) { }

        public bool addNewDestination(Destination destination)
        {
            return false;
        }

        public Destination getDestinationInfoByID(Guid destinationID){            
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.destinationID == destinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>();
        }

        public bool updateDestination(Destination destination) 
        {
            var query = Db.Cypher.Match("(destinationTaken:Destination)").Where((Destination destinationTaken) => destinationTaken.destinationID == destination.destinationID).
                Set("destinationTaken = {destination}").WithParams(new { destination }).Return(destinationReturned => destinationReturned.As<Destination>()).Results;
            return (query.First<Destination>() != null);
        }

        public int getNumberOfLikes(Guid destinationID){
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.destinationID == destinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>().numberOfLikes;
        }        
    }

    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        Destination getDestinationInfoByID(Guid destinationID);

        int getNumberOfLikes(Guid destinationID);

        bool addNewDestination(Destination destination);
    }

}
