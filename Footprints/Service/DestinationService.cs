using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Service
{
    public interface IDestinationService { 
    }
    public class DestinationService : IDestinationService
    {
        IDestinationService _destinationRepo;
        public DestinationService(IDestinationService destinationRepo) {
            _destinationRepo = destinationRepo;
        }
    }
}