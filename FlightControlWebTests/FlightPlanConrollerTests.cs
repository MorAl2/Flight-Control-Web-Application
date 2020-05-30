using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using FlightControlWeb.Models;
using FlightControlWeb.Controllers;
using System.Collections.Generic;

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
            var result = flightPlanTester.IsValidFlightPlan(flightPlan);
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidFlightPlan_NotFullFlight_ReturnsFalse()
        {
            // Arrange
            var flightPlanTester = new FlightPlanController(new FlightControlManager());
            // Act
            var result = flightPlanTester.IsValidFlightPlan(new FlightPlan());
            // Assert
            Assert.IsFalse(result);
        }


    }
}
