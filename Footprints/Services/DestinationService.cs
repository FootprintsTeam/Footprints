using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;

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

        public Destination getDestinationInfoByID(String destinationID)
        {
            return _destinationRepo.getDestinationInfoByID(destinationID);
        }
    }
}