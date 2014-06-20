using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Footprints.DAL.Abstract;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Footprints.Service;
using Footprints.Controllers;
using Moq;

namespace Footprints.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateDbConnection()
        {
            //Arrange            
            Mock<IJourneyRepository> testObj = new Mock<IJourneyRepository>();
            //Act
            var x = testObj.Object;
            //Assert
            Assert.IsNotNull(x);
        }

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
                Destination { name = DESTINATION_NAME, description = DESCRIPTION, longitude = LONGTITUDE, latitude = LATITUDE, numberOfLikes = numberOfLikes });
            var service = new DestinationService(repository.Object);

            //--2. Act ---
            var destination = service.getDestinationInfoByID(DESTINATION_ID);
            //--3. Assert ---
            Assert.IsTrue(destination != null);
            Assert.IsTrue(destination.name == DESTINATION_NAME);
            Assert.IsTrue(destination.description == DESCRIPTION);
        }
    }
}
