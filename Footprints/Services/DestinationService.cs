using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.Service
{
    public interface IDestinationService {
        bool AddDestination(Destination destination);
        Destination GetDestinationDetail(Guid destinationID);
        int GetNumberOfLike(Guid destinationID);
        bool UpdateDestination(Destination destination);
    }
    public class DestinationService : IDestinationService
    {
        IDestinationRepository _destinationRepo;
        public DestinationService(IDestinationRepository destinationRepo) {
            _destinationRepo = destinationRepo;
        }

        public bool AddDestination(Destination destination)
        {
           return _destinationRepo.AddNewDestination(destination);
        }

        public Destination GetDestinationDetail(Guid destinationID)
        {
            return _destinationRepo.GetADestination(destinationID);
        }

        public int GetNumberOfLike(Guid destinationID)
        {
            return _destinationRepo.GetNumberOfLike(destinationID);
        }

        public bool UpdateDestination(Destination destination) {
            return _destinationRepo.UpdateDestination(destination);
        }
    }
}