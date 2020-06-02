using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FlightControlWeb.Models
{
    public class FlightControlManager : IFlightManager
    {
        // dictionary for holding the local FLight Plans. id to flightplan
        private ConcurrentDictionary<string, FlightPlan> flightPlans;
        // dictionary for holding the Externtal Servers. id to server
        private ConcurrentDictionary<string, Server> servers;
        // dictioprnay mapping external flight plans to their External server. id to server
        private ConcurrentDictionary<string, Server> externalFlightsIndex;

        public FlightControlManager()
        {
            flightPlans = new ConcurrentDictionary<string, FlightPlan>();
            servers = new ConcurrentDictionary<string, Server>();
            externalFlightsIndex = new ConcurrentDictionary<string, Server>();
        }

        // adding a new FlightPlan To the dictioanry
        public void AddFlightPlan(FlightPlan flight)
        {
            // giving uniqe Id to the FlightPlan
            string id = this.ProvideFlightId(flight);
            flight.Flight_Id = id;
            // trying to add the flight to the list.
            flightPlans.TryAdd(id, flight);
        }

        //adding a new External Server.
        public void addServer(Server s)
        {
            servers.TryAdd(s.ServerId, s);
        }

        // getting a specific flightplan
        public async Task<FlightPlan> GetFlightPlan(string id)
        {
            // Holding the wanted FlightPlan
            FlightPlan res;
            // Holding the specific external server
            Server serverExt;
            // connecting to external Servers.
            HttpClient httpClient = new HttpClient();
            // trying to get the flightpaln Localy.
            if (flightPlans.TryGetValue(id, out res))
            {
                return res;
            }
            // checking if the id was rcved from external servers.
            else if (externalFlightsIndex.TryGetValue(id, out serverExt))
            {
                try
                {
                    // getting the flight plan.
                    return await GetFlightPlanExternal(id, serverExt, httpClient);
                }
                catch
                {
                    // the server dosen't responded so no flightplan was found.
                    return null;
                }
            }
            // if wasn't found internal or External.
            return null;
        }

        // getting the id from the extrenal server.
        private async Task<FlightPlan> GetFlightPlanExternal(string id, Server serverExt,
            HttpClient httpClient)
        {
            // var for holding the server response.
            HttpResponseMessage responseMessage;
            // validating the url format and sending a request.
            if (serverExt.ServerURL[serverExt.ServerURL.Length - 1] == '/')
            {
                responseMessage = await httpClient.GetAsync(serverExt.ServerURL +
                    "api/FlightPlan/" + id);
            }
            else
            {
                responseMessage = await httpClient.GetAsync(serverExt.ServerURL +
                    "/api/FlightPlan/" + id);
            }
            // if the sever returned HTTP error.
            if (!responseMessage.IsSuccessStatusCode)
                return null;
            var strdata = await responseMessage.Content.ReadAsStringAsync();
            // creating a json object and then converting to flight plan.
            JObject jsonPlan = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(strdata);
            FlightPlan tempFlightPlan = new FlightPlan();
            CreateFlightPlan(id, jsonPlan, tempFlightPlan);
            // returning the result flightPlan if it's valid.
            if (tempFlightPlan.IsValidFlightPlan())
            {
                return tempFlightPlan;
            }
            return null;
        }

        private void CreateFlightPlan(string id, JObject jsonFlightPlan, FlightPlan tempFlightPlan)
        {
            tempFlightPlan.Company_Name = (string)jsonFlightPlan["company_name"];
            tempFlightPlan.Flight_Id = id;
            tempFlightPlan.Initial_Location = new LocationAndTime();
            tempFlightPlan.Initial_Location.Latitude =
                (double)jsonFlightPlan["initial_location"]["latitude"];
            tempFlightPlan.Initial_Location.Longitude =
                (double)jsonFlightPlan["initial_location"]["longitude"];
            tempFlightPlan.Initial_Location.StartTime =
                (DateTime)jsonFlightPlan["initial_location"]["date_time"];
            tempFlightPlan.Initial_Location.StartTime =
                tempFlightPlan.Initial_Location.StartTime.ToUniversalTime();
            tempFlightPlan.Passengers = (int)jsonFlightPlan["passengers"];
            tempFlightPlan.Segments = new List<Segment>();
            // adding each segment
            foreach (var segment in jsonFlightPlan["segments"])
            {
                Segment segmentTemp = new Segment();
                segmentTemp.Latitude = (double)segment["latitude"];
                segmentTemp.Longitude = (double)segment["longitude"];
                segmentTemp.Timespan_seconds = (int)segment["timespan_seconds"];
                if (!segmentTemp.IsValidSegment())
                    tempFlightPlan.Segments.Add(segmentTemp);
            }
        }

        // getting all the flights relevent to the time rcved.
        public async Task<IEnumerable<Flight>> GetFlights(DateTime rel, bool sync_all)
        {
            HttpClient httpClient = new HttpClient();
            // holding the relevent Flights.
            List<Flight> flights = new List<Flight>();
            // going over the local flight plans and checking if the time is right.
            foreach (FlightPlan plan in this.flightPlans.Values)
            {
                if (plan.GetLandingTime().CompareTo(rel) >= 0
                    && plan.Initial_Location.StartTime.CompareTo(rel) <= 0)
                {
                    // creating a flight from the flight plan.
                    Flight flight = new Flight(plan, false);
                    LocationAndTime Current = plan.GetCurrentLocation(rel);
                    flight.Longitude = Current.Longitude;
                    flight.Latitude = Current.Latitude;
                    flights.Add(flight);
                }
            }
            if (sync_all)
            {
                // going over all the external servers.
                foreach (Server server in servers.Values)
                {
                    // adding to flights list all the relevevnt external flights.
                    await GetFlightsExternal(rel, httpClient, flights, server);
                }
            }
            // ordering the flight by the internal and then the external.
            return flights.OrderBy(e => e.Is_external ? 1 : 0);
        }

        private async Task GetFlightsExternal(DateTime rel, HttpClient client,
            List<Flight> flights, Server server)
        {
            try
            {
                HttpResponseMessage responseMessage;
                if (server.ServerURL[server.ServerURL.Length - 1] == '/')
                {
                    string dta = "api/Flights?relative_to=" + rel.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    responseMessage = await client.GetAsync(server.ServerURL + dta);
                }
                else
                {
                    string dta = "/api/Flights?relative_to=" + rel.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    responseMessage = await client.GetAsync(server.ServerURL + dta);
                }
                // if the server returned error then go to the next server.
                if (!responseMessage.IsSuccessStatusCode)
                {
                    return;
                }
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                JArray jsonArray;
                jsonArray = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(responseString);
                CreateFlightFromJson(flights, server, jsonArray);
            }
            catch
            {
                return;
            }
        }

        private void CreateFlightFromJson(List<Flight> flights, Server server,
            JArray jsonFlightsArray)
        {
            // going over each json flight object and creating flights.
            foreach (var flight in jsonFlightsArray)
            {
                // creating a new flight and filling it.
                Flight tempFlight = new Flight();
                tempFlight.Company_Name = (string)flight["company_name"];
                tempFlight.Flight_id = (string)flight["flight_id"];
                tempFlight.Is_external = true;
                tempFlight.Latitude = (double)flight["latitude"];
                tempFlight.Longitude = (double)flight["longitude"];
                tempFlight.Passengers = (int)flight["passengers"];
                tempFlight.TakeOffTime = (DateTime)flight["date_time"];
                tempFlight.TakeOffTime = tempFlight.TakeOffTime.ToUniversalTime();
                // checking if rcved a valid flight.
                if (tempFlight.IsValidFlight())
                {
                    // adding to the flights and mapping id to server.
                    flights.Add(tempFlight);
                    externalFlightsIndex.TryAdd(tempFlight.Flight_id, server);
                }
            }
        }

        // get all the registered servers
        public IEnumerable<Server> GetServers()
        {
            return this.servers.Values;
        }

        // remove s erver from the list
        public bool RemoveServer(string id)
        {
            // deleting the mappings to the servers.
            foreach (KeyValuePair<string, Server> pair in externalFlightsIndex)
            {
                if (pair.Value.ServerId == id)
                {
                    externalFlightsIndex.TryRemove(pair.Key, out _);
                }
            }
            // removing the servers.
            return this.servers.TryRemove(id, out _);

        }

        // remove flightplan from the list.
        public bool RemoveFlightPlan(string id)
        {
            return flightPlans.TryRemove(id, out _);
        }

        // creating a uniqe ID for the flights
        public string ProvideFlightId(FlightPlan flightPlan)
        {
            // basing the Id on the Company Name
            string id = "";
            string s = new string(flightPlan.Company_Name);
            s = s.Replace("-", " ");
            string[] words = s.Split(" ");
            if (words.Length >= 2)
            {
                id += words[0][0].ToString() + words[1][0].ToString();
            }
            else if (words.Length == 1)
            {
                if (words[0].Length >= 2)
                {
                    id += words[0][0].ToString() + words[0][1].ToString().ToUpper();
                }
            }
            id += RandomAlphanmeric(8 - id.Length);
            return id;
        }

        // returning random String.
        private string RandomAlphanmeric(int length)
        {
            Random random = new Random();
            string result = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (length == 8)
            {
                result += chars[random.Next(0, chars.Length)].ToString() +
                    chars[random.Next(0, chars.Length)].ToString();
                length = 6;
            }
            int size = random.Next(1, 3);
            if (size == 2)
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
