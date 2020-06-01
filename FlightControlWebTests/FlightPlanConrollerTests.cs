using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using FlightControlWeb.Models;
using FlightControlWeb.Controllers;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FlightControlWeb.Models;
using FlightControlWeb.Controllers;
using System.Collections.Generic;
using Autofac.Extras.Moq;
using System.Threading.Tasks;

namespace FlightControlWebTests
{
    
    [TestClass]
    public class FlightPlanConrollerTests
    {
        [TestMethod]
        public void IsValidFlightPlan_FullFlight_ReturnsTrue()
        {
            // Arrange
            //new FlightPlan.
            var flightPlan = new FlightPlan();

            //new segment list.
            List<Segment> list = new List<Segment>();
            var segments = new Segment();
            segments.Latitude = 31.12;
            segments.Longitude = 33.16;
            segments.Timespan_seconds = 500;
            list.Add(segments);
            //new location.
            var location = new LocationAndTime(16, 14, new DateTime());
            
            //initial our flightPlan
            //flightPlan.Flight_Id = "test12";
            flightPlan.Passengers = 50;
            flightPlan.Company_Name = "comp-name";
            flightPlan.Initial_Location = location;
            flightPlan.Segments = list;

            //creating flightPlanController
            var flightPlanTester = new FlightPlanController(new FlightControlManager());
             
            // Act
            var result = flightPlan.IsValidFlightPlan();
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidFlightPlan_NotFullFlight_ReturnsFalse()
        {
            // Arrange
            var flightPlanTester = new FlightPlanController(new FlightControlManager());
            // Act
            var result = new FlightPlan().IsValidFlightPlan();
            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void GetPlanByIDTest_ValidData_ReturnsFlightPlan()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var testID = "12345";
                mock.Mock<IFlightManager>()
                    .Setup(x => x.GetFlightPlan(testID))
                    .Returns(getSampleFlightPlan(testID));

                var controllerTester = mock.Create<FlightPlanController>();

                var expected = getSampleFlightPlan(testID);

                var actual = controllerTester.GetFlightPlan(testID);

                Assert.IsTrue(actual != null);
                Assert.AreEqual(expected.Result.Flight_Id, actual.Result.Flight_Id);
                Assert.AreEqual(expected.Result.Company_Name, actual.Result.Company_Name);
                Assert.AreEqual(expected.Result.Passengers, actual.Result.Passengers);
            }
        }

        private async Task<FlightPlan> getSampleFlightPlan(string id)
        {
            var flightPlan = new FlightPlan();

            //new segment list.
            List<Segment> list = new List<Segment>();
            var segments = new Segment();
            segments.Latitude = 31.12;
            segments.Longitude = 33.16;
            segments.Timespan_seconds = 500;
            list.Add(segments);
            //new location.
            var location = new LocationAndTime(16, 14, new DateTime());

            //initial our flightPlan
            //flightPlan.Flight_Id = "test12";
            flightPlan.Flight_Id = id;
            flightPlan.Passengers = 50;
            flightPlan.Company_Name = "comp-name";
            flightPlan.Initial_Location = location;
            flightPlan.Segments = list;

            return flightPlan;
        }


    }
}
