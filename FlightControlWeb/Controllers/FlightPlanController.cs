using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Dependency Injection of the Control Manager.
    public class FlightPlanController : ControllerBase
    {
        private readonly IFlightManager flightControlManager;
        public FlightPlanController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        // getting FlightPlan By Id.
        public async Task<FlightPlan> GetFlightPlan(string id)
        {
            return await this.flightControlManager.GetFlightPlan(id);
        }

        // POST: api/FlightPlan
        [HttpPost]
        // adding a new FlightPlan.
        public ActionResult AddFlightPlan([FromBody]FlightPlan flightPlan)
        {
            // checking if valid.
            if (flightPlan.IsValidFlightPlan())
            {
                flightPlan.Initial_Location.StartTime = flightPlan.Initial_Location.StartTime.ToUniversalTime();
                this.flightControlManager.AddFlightPlan(flightPlan);
                return Ok("Flight Id: " + flightPlan.Flight_Id);
            }
            return BadRequest("Invalid FlightPlan");

        }
    }
}
