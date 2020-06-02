using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
        // test for IsValidFlightPlan function.
        // in this scenario - we are adding legal flight and expect to get true as a return value.
        public void IsValidFlightPlanFullFlightReturnsTrue()
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
        // test for IsValidFlightPlan function.
        // in this scenario, we are adding illegal flight and expect to get true as a return value.
        public void IsValidFlightPlanNotFullFlightReturnsFalse()
        {
            // Arrange
            var flightPlanTester = new FlightPlanController(new FlightControlManager());
            // Act
            var result = new FlightPlan().IsValidFlightPlan();
            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        // test for the GetPlanByIDTest, using mock to mocking the IFlightManager interface object.
        // by creating a fake FlightPlanController we check if we can get a whole flight plan,
        // using asynchronic task.
        // in this scenario - a success will be:
        // 1. get a non null object.
        // 2. get identical flight plan from our fake function and the mock's one.
        public void GetPlanByIDTestValidDataReturnsFlightPlan()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var testID = "12345";
                mock.Mock<IFlightManager>()
                    .Setup(x => x.GetFlightPlan(testID))
                    .Returns(GetSampleFlightPlan(testID));

                var controllerTester = mock.Create<FlightPlanController>();

                var expected = GetSampleFlightPlan(testID);

                var actual = controllerTester.GetFlightPlan(testID);

                Assert.IsTrue(actual != null);
                
                Assert.AreEqual(expected.Result.Flight_Id, actual.Result.Value.Flight_Id);
                Assert.AreEqual(expected.Result.Company_Name, actual.Result.Value.Company_Name);
                Assert.AreEqual(expected.Result.Passengers, actual.Result.Value.Passengers);
            }
        }

        // mocking the GetFlightPlan of FlightPlanController class.
        private async Task<FlightPlan> GetSampleFlightPlan(string id)
        {
            return await Task.Run(() => GetAsyncSampleFlightPlan(id));
        }

        // a help function - called from the task of GetSampleFlightPlan
        private FlightPlan GetAsyncSampleFlightPlan(string id)
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

            flightPlan.Flight_Id = id;
            flightPlan.Passengers = 50;
            flightPlan.Company_Name = "comp-name";
            flightPlan.Initial_Location = location;
            flightPlan.Segments = list;

            return flightPlan;
        }

    }
}
