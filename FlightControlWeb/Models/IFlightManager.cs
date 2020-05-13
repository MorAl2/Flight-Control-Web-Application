using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        public void AddFlightPlan(FlightPlan flight);
        public void addServer(Server s);
        public FlightPlan GetFlightPlan(string id);
        public IEnumerable<Flight> GetFlights(DateTime rel, bool sync_all);
        public IEnumerable<Server> GetServers();
        public void RemoveServer(string id);
        public void RemoveFlightPlan(string id);
        public string ProvideFlightId(FlightPlan flightPlan);
    }
}
