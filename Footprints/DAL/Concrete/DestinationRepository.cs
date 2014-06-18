using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
using Footprints.DAL.Concrete;

namespace Footprints.DAL.Concrete
{
    public class DestinationRepository : RepositoryBase<DestinationRepository>, IDestinationRepository
    {
        public DestinationRepository(IGraphClient client) : base(client) { }

        public Destination getDestinationInfoByID(String destinationID){
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.destinationID == destinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>();
        }

        public int getNumberOfLikes(String destinationID){
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.destinationID == destinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>().numberOfLikes;
        }        
    }

    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        Destination getDestinationInfoByID(String destinationID);

        int getNumberOfLikes(String destinationID);
    }

}
