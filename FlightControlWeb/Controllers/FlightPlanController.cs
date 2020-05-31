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
    public class FlightPlanController : ControllerBase
    {
        private readonly IFlightManager flightControlManager;
        public FlightPlanController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<FlightPlan> GetFlightPlan(string id)
        {
            return await this.flightControlManager.GetFlightPlan(id);
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult AddFlightPlan([FromBody]FlightPlan flightPlan)
        {
            if (IsValidFlightPlan(flightPlan))
            {
                flightPlan.Initial_Location.StartTime = flightPlan.Initial_Location.StartTime.ToUniversalTime();
                this.flightControlManager.AddFlightPlan(flightPlan);
                return Ok("Flight Id: " + flightPlan.Flight_Id);
            }
            return BadRequest("Invalid FlightPlan");

        }

        public Boolean IsValidFlightPlan(FlightPlan plan)
        {
            if(plan.Company_Name!=null&& plan.Initial_Location!=null && plan.Passengers!=-1 && plan.Segments!= null)
            {
                if(plan.Initial_Location.Latitude!=Double.NaN && plan.Initial_Location.Longitude != Double.NaN && plan.Initial_Location.StartTime != null)
                return true;
            }

            return false;
        }
    }
}
