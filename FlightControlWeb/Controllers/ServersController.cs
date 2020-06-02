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
        // Dependency Injection of the Control Manager.
        private readonly IFlightManager flightControlManager;
        public ServersController(IFlightManager flight)
        {
            this.flightControlManager = flight;
        }

        // GET: api/Servers
        [HttpGet]
        // getting all the servers
        public IEnumerable<Server> GetServers()
        {
            return this.flightControlManager.GetServers();
        }

        // POST: api/Servers
        [HttpPost]
        // adding a server and checking if valid.
        public ActionResult AddServer([FromBody] Server server)
        {
            if (server.IsValidServer())
            {
                this.flightControlManager.AddServer(server);
                return Ok("Server Added");
            }
            return BadRequest("Invalid Server");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        // deleting extenal server by id.
        public ActionResult DeleteServer(string id)
        {
            if (this.flightControlManager.RemoveServer(id))
            {
                return Ok("Server Removed");
            }
            else {
                return BadRequest("ID Wasn't Found");
            }
        }
    }
}
