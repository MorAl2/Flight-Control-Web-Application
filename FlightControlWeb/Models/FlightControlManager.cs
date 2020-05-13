using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;

namespace FlightControlWeb.Models
{
    public class FlightControlManager : IFlightManager
    {
        private ConcurrentDictionary<string,FlightPlan> flightPlans = new ConcurrentDictionary<string, FlightPlan>();
        private ConcurrentDictionary<string, Server> servers = new ConcurrentDictionary<string, Server>();
        private readonly HttpClient httpClient = new HttpClient();

        public FlightControlManager()
        {

        }
        public void AddFlightPlan(FlightPlan flight)
        {
            string id = this.ProvideFlightId(flight);
            flight.Flight_Id = id;
            flightPlans.TryAdd(id, flight);
        }

        public void addServer(Server s)
        {
            servers.TryAdd(s.ServerId,s);
        }

        public FlightPlan GetFlightPlan(string id)
        {
            FlightPlan res;
            if (flightPlans.TryGetValue(id, out res))
            {
                return res;
            }
            return new FlightPlan();
        }
        public IEnumerable<Flight> GetFlights(DateTime rel, bool sync_all)
        {
            List<Flight> flights = new List<Flight>();
            if (sync_all)
            {
                foreach(Server server in servers.Values)
                {
                    //var responseString = await client.GetStringAsync();
                }
            }
            foreach (FlightPlan plan in this.flightPlans.Values)
            {
                if (plan.GetLandingTime().CompareTo(rel) >= 0 && plan.Initial_Location.StartTime.CompareTo(rel)<=0)
                {
                    Flight flight = new Flight(plan, false);
                    LocationAndTime Current = plan.GetCurrentLocation(rel);
                    flight.Longitude = Current.longitude;
                    flight.Latitude = Current.latitude;
                    flights.Add(flight);
                }
            }
            return flights;
        }

        public IEnumerable<Server> GetServers()
        {
            return this.servers.Values;
        }

        public void RemoveServer(string id)
        {
            this.servers.TryRemove(id,out _);
        }

        public void RemoveFlightPlan(string id)
        {
            flightPlans.TryRemove(id,out _);
        }

        public string ProvideFlightId(FlightPlan flightPlan)
        {
            string id = "";
            string s = new string(flightPlan.Company_Name);
            s = s.Replace("-"," ");
            string[] words = s.Split(" ");
            if(words.Length >= 2)
            {
                id += words[0][0].ToString() + words[1][0].ToString();
            }
            else if(words.Length == 1)
            {
                if (words[0].Length >= 2)
                {
                    id += words[0][0].ToString() + words[0][1].ToString();
                }
            }
            id += RandomAlphanmeric(8 - id.Length);
            return id;
        }

        private string RandomAlphanmeric(int length)
        {
            Random random = new Random();
            string result = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (length == 8)
            {
                result += chars[random.Next(0, chars.Length)].ToString() + chars[random.Next(0, chars.Length)].ToString();
                length = 6;
            }
            int size = random.Next(1, 3);
            if(size == 2)
            {
                result += random.Next(1000, 10000).ToString();
            }
            else
            {
                result += random.Next(100, 1000).ToString();
            }
            result += chars[random.Next(0, chars.Length)].ToString();
            return result;
        }
    }
}
