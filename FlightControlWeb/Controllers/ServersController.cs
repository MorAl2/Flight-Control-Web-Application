using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/servers")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly IFlightManager flightControlManager;
        public ServersController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }


        // GET: api/Servers
        [HttpGet]
        public IEnumerable<Server> GetServers()
        {
            return this.flightControlManager.GetServers();
        }

        // POST: api/Servers
        [HttpPost]
        public ActionResult AddServer([FromBody] Server server)
        {
            if (server.IsValidServer())
            {
                this.flightControlManager.addServer(server);
                return Ok("Server Added");
            }
            return NotFound("Invalid Server");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteServer(string id)
        {
            this.flightControlManager.RemoveServer(id);
        }
    }
}
