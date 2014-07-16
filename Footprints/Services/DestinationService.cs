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
    }
    public class DestinationService : IDestinationService
    {
        IDestinationRepository _destinationRepo;
        public DestinationService(IDestinationRepository destinationRepo) {
            _destinationRepo = destinationRepo;
        }

        public bool addNewDestination(Destination destination)
        {
           return _destinationRepo.AddNewDestination(destination);
        }

        public Destination getDestinationInfoByID(Guid destinationID)
        {
            return _destinationRepo.GetDestinationDetail(destinationID);
        }

        public int getNumberOfLikes(Guid destinationID)
        {
            return _destinationRepo.GetNumberOfLike(destinationID);
        }
    }
}