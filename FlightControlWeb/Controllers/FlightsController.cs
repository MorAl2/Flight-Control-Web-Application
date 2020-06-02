using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Dependency Injection of the Control Manager.
    public class FlightsController : ControllerBase
    {
        private readonly IFlightManager flightControlManager;
        public FlightsController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }

        // GET: api/Flights
        [HttpGet]
        // getting all the relevent flights.
        public async Task<IEnumerable<Flight>> GetFlights([FromQuery] DateTime relative_to)
        {
            relative_to = relative_to.ToUniversalTime();
            bool sync = Request.QueryString.Value.Contains("sync_all");
            return await flightControlManager.GetFlights(relative_to, sync);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        // deleting a flight.
        public ActionResult DeleteFlight(string id)
        {
            if (this.flightControlManager.RemoveFlightPlan(id))
            {
                return Ok("Flight Deleted");
            }
            else
            {
                return BadRequest("ID Wasn't Found");
            }
        }
    }
}
