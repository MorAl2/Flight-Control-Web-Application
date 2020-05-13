using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightManager flightControlManager;
        public FlightsController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }

        // GET: api/Flights
        [HttpGet]
        public IEnumerable<Flight> GetFlights([FromQuery] DateTime relative_to)
        {
            relative_to = relative_to.ToUniversalTime();
            bool sync = Request.QueryString.Value.Contains("sync_all");
            return this.flightControlManager.GetFlights(relative_to, sync);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteFlight(string id)
        {
            this.flightControlManager.RemoveFlightPlan(id);
        }
    }
}
