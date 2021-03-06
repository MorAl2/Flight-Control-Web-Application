﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        public void AddFlightPlan(FlightPlan flight);
        public void AddServer(Server s);
        public Task<FlightPlan> GetFlightPlan(string id);
        public Task<IEnumerable<Flight>> GetFlights(DateTime rel, bool sync_all);
        public IEnumerable<Server> GetServers();
        public bool RemoveServer(string id);
        public bool RemoveFlightPlan(string id);
        public string ProvideFlightId(FlightPlan flightPlan);
    }
}
