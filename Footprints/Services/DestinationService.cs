﻿using System;
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

        public CypherNet.Graph.Node addNewDestination(Destination destination)
        {
            return _destinationRepo.addNewDestination(destination);
        }

        public Destination getDestinationInfoByID(String destinationID)
        {
            return _destinationRepo.getDestinationInfoByID(destinationID);
        }

        public int getNumberOfLikes(String destinationID)
        {
            return _destinationRepo.getNumberOfLikes(destinationID);
        }
    }
}