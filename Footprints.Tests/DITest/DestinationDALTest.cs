﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Footprints.DAL.Abstract;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Footprints.Service;
namespace Footprints.Tests.DITest
{
    [TestClass]
    public class DestinationDALTest
    {
        [TestMethod]
        public void getDestinationInfoByID()
        {
            //--1. Arrange ---
            const String DESTINATION_ID = "1";
            const String DESTINATION_NAME = "Footprints 1";
            const int numberOfLikes = 0;
            const String DESCRIPTION = "Footprints 1";
            const double LATITUDE = 105.838938;
            const double LONGTITUDE = 21.036476;
            
            var repository = new Mock<IDestinationRepository>();
            repository.Setup(m => m.getDestinationInfoByID(DESTINATION_ID)).Returns(new 
                Destination { name = DESTINATION_NAME, description = DESCRIPTION, longitude = LONGTITUDE, latitude = LATITUDE, numberOfLikes = numberOfLikes});
            var service = new DestinationService(repository.Object);
            //--2. Act ---
            var destination = service.getDestinationInfoByID(DESTINATION_ID);
            //--3. Assert ---
            Assert.IsTrue(destination != null);
            Assert.IsTrue(destination.name == DESTINATION_NAME);
            Assert.IsTrue(destination.description == DESCRIPTION);
        }

        [TestMethod]
        public void getNumberOfLikes()
        {
            //--1. Arrange ---

            //--2. Act ---

            //--3. Assert ---
        }

        [TestMethod]
        public void addNewDestination() 
        {
            //--1. Arrange ---
            Destination destination = new Destination
            {
                destinationID = "2",
                name = "Footprints 2",
                description = "Footprints 2",
                longitude = 105.838938,
                latitude = 21.036476,
                takenDate = DateTime.Today,
                numberOfLikes = 0,
                timestamp = DateTime.Today,             
            };
            var repository = new Mock<IDestinationRepository>();
            repository.Setup(m => m.addNewDestination(destination));
            var service = new DestinationService(repository.Object);
            //--2. Act ---
            var node = service.addNewDestination(destination);
            //--3. Assert ---
            repository.Verify(s => s.addNewDestination(destination));
            Assert.IsTrue(node != null);
        }

    }


}
