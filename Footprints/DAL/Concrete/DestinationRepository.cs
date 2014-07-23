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
        public Destination GetADestination(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>();
        }
        //TODO
        public bool AddNewDestination(Destination Destination, String PlaceID, Guid JourneyID)
        {

            return false;
        }

        public bool UpdateDestination(Destination Destination)
        {
            var query = Db.Cypher.Match("(destinationTaken:Destination)").Where((Destination destinationTaken) => destinationTaken.DestinationID == Destination.DestinationID).
                Set("destinationTaken = {destination}").WithParams(new { destination = Destination }).Return(destinationReturned => destinationReturned.As<Destination>()).Results;
            return (query.First<Destination>() != null);
        }

        //TODO
        public void DeleteDestination(Guid DestinationID)
        {

        }

        //TODO
        public void AddNewContent(Content Content, Guid DestinationID)
        {

        }
        public void UpdateContent(Content Content)
        {
            Db.Cypher.Match("(Content:Content)").Where((Content content) => content.ContentID == Content.ContentID).
                        Set("Content = {Content}").WithParam("Content", Content).ExecuteWithoutResults();
        }
        //TODO
        public void DeleteContent(Guid ContentID)
        {

        }

        public IEnumerable<Content> GetAllContent(Guid DestinationID)
        {
            return Db.Cypher.Match("(Destination:Destination)-[:HAS]->(Content:Content)").Where((Destination Destination) => Destination.DestinationID == DestinationID).Return(content => content.As<Content>()).Results;
        }

        public void AddNewPlace(Place Place)
        {
            Db.Cypher.Create("(Place:Place {Place})").WithParam("Place", Place).ExecuteWithoutResults();
        }
        public int GetNumberOfLike(Guid DestinationID)
        {
            var query = Db.Cypher.Match("(destination:Destination)").Where((Destination destination) => destination.DestinationID == DestinationID).Return(destination => destination.As<Destination>());
            return query.Results.First<Destination>().NumberOfLike;
        }

        public void LikeDestination(Guid DestinationID) 
        { 
        }

        public void UnlikeDestination(Guid UserID, Guid DestinationID)
        {

        }

        public int GetNumberOfShare(Guid DestinationID)
        {
            return 0;
        }

        public void ShareDestination(Guid UserID,Guid DestinationID, String Content)
        {

        }


        public bool AddNewDestination(Destination destination)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDestinationRepository : IRepository<DestinationRepository>
    {
        Destination GetADestination(Guid destinationID);

        int GetNumberOfLike(Guid destinationID);

        bool AddNewDestination(Destination destination);

        bool UpdateDestination(Destination destination);
        void DeleteDestination(Guid DestinationID);
        void AddNewContent(Content Content, Guid DestinationID);
        void UpdateContent(Content Content);
        void DeleteContent(Guid ContentID);
        IEnumerable<Content> GetAllContent(Guid DestinationID);
        void AddNewPlace(Place Place);
    }

}
