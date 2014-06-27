using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient;
using CypherNet;
using Footprints.DAL.Concrete;

namespace Footprints.DAL.Concrete
{
    public class DestinationRepository : RepositoryBase<DestinationRepository>, IDestinationRepository
    {
        public DestinationRepository(IGraphClient client) : base(client) { }

        public CypherNet.Graph.Node addNewDestination(Destination destination){
            var clientFactory = CypherNet.Configuration.Fluently.Configure("http://54.255.155.78:7474/db/data").CreateSessionFactory();
            var cypherEndpoint = clientFactory.Create();
            CypherNet.Graph.Node destinationNode;
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                destinationNode = cypherEndpoint.CreateNode(destination, "Destination");
                transaction.Complete();
            }
            return destinationNode;
        }

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

        CypherNet.Graph.Node addNewDestination(Destination destination);
    }

}
