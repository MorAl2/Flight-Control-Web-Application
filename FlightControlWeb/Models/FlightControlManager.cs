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
        private ConcurrentDictionary<string, FlightPlan> flightPlans = new ConcurrentDictionary<string, FlightPlan>();
        private ConcurrentDictionary<string, Server> servers = new ConcurrentDictionary<string, Server>();
        private ConcurrentDictionary<string, Server> externalFlightsIndex = new ConcurrentDictionary<string, Server>();
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
            servers.TryAdd(s.ServerId, s);
        }

        public async Task<FlightPlan> GetFlightPlan(string id)
        {
            FlightPlan res;
            Server serverExt;
            HttpClient httpClient = new HttpClient();
            if (flightPlans.TryGetValue(id, out res))
            {
                return res;
            }
            else if (externalFlightsIndex.TryGetValue(id, out serverExt))
            {
                try
                {
                    HttpResponseMessage responseMessage;
                    if (serverExt.ServerURL[serverExt.ServerURL.Length - 1] == '/')
                    {
                        responseMessage = await httpClient.GetAsync(serverExt.ServerURL + "api/FlightPlan/" + id);
                    }
                    else
                    {
                        responseMessage = await httpClient.GetAsync(serverExt.ServerURL + "/api/FlightPlan/" + id);
                    }
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var responseString = await responseMessage.Content.ReadAsStringAsync();
                        JObject jsonFlightPlan = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(responseString);
                        FlightPlan tempFlightPlan = new FlightPlan();
                        tempFlightPlan.Company_Name = (string)jsonFlightPlan["company_name"];
                        tempFlightPlan.Flight_Id = (string)jsonFlightPlan["flight_Id"];
                        tempFlightPlan.Initial_Location = new LocationAndTime();
                        tempFlightPlan.Initial_Location.Latitude = (double)jsonFlightPlan["initial_location"]["latitude"];
                        tempFlightPlan.Initial_Location.Longitude = (double)jsonFlightPlan["initial_location"]["longitude"];
                        tempFlightPlan.Initial_Location.StartTime = (DateTime)jsonFlightPlan["initial_location"]["date_time"];
                        tempFlightPlan.Initial_Location.StartTime = tempFlightPlan.Initial_Location.StartTime.ToUniversalTime();
                        tempFlightPlan.Passengers = (int)jsonFlightPlan["passengers"];
                        tempFlightPlan.Segments = new List<Segment>();
                        foreach (var segment in jsonFlightPlan["segments"])
                        {
                            Segment segmentTemp = new Segment();
                            segmentTemp.Latitude = (double)segment["latitude"];
                            segmentTemp.Longitude = (double)segment["longitude"];
                            segmentTemp.Timespan_seconds = (int)segment["timespan_seconds"];
                            tempFlightPlan.Segments.Add(segmentTemp);
                        }
                        return tempFlightPlan;
                    }
                }
                catch
                {
                    return new FlightPlan();
                }
            }
            return new FlightPlan();
        }
        public async Task<IEnumerable<Flight>> GetFlights(DateTime rel, bool sync_all)
        {
            HttpClient httpClient = new HttpClient();
            List<Flight> flights = new List<Flight>();
            foreach (FlightPlan plan in this.flightPlans.Values)
            {
                if (plan.GetLandingTime().CompareTo(rel) >= 0 && plan.Initial_Location.StartTime.CompareTo(rel) <= 0)
                {
                    Flight flight = new Flight(plan, false);
                    LocationAndTime Current = plan.GetCurrentLocation(rel);
                    flight.Longitude = Current.Longitude;
                    flight.Latitude = Current.Latitude;
                    flights.Add(flight);
                }
            }
            if (sync_all)
            {
                foreach (Server server in servers.Values)
                {
                    try
                    {
                        HttpResponseMessage responseMessage;
                        if (server.ServerURL[server.ServerURL.Length - 1] == '/')
                        {
                            responseMessage = await httpClient.GetAsync(server.ServerURL + "api/Flights?relative_to=" + rel.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                        }
                        else
                        {
                            responseMessage = await httpClient.GetAsync(server.ServerURL + "/api/Flights?relative_to=" + rel.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                        }

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var responseString = await responseMessage.Content.ReadAsStringAsync();
                            JArray jsonFlightsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(responseString);
                            foreach (var flight in jsonFlightsArray)
                            {
                                Flight tempFlight = new Flight();
                                tempFlight.Company_Name = (string)flight["company_name"];
                                tempFlight.Flight_id = (string)flight["flight_id"];
                                tempFlight.Is_external = true;
                                tempFlight.Latitude = (double)flight["latitude"];
                                tempFlight.Longitude = (double)flight["longitude"];
                                tempFlight.Passengers = (int)flight["passengers"];
                                tempFlight.TakeOffTime = (DateTime)flight["date_time"];
                                tempFlight.TakeOffTime = tempFlight.TakeOffTime.ToUniversalTime();
                                flights.Add(tempFlight);
                                externalFlightsIndex.TryAdd(tempFlight.Flight_id, server);
                            }
                        }
                    }
                    catch
                    {

                    }
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
            this.servers.TryRemove(id, out _);
        }

        public void RemoveFlightPlan(string id)
        {
            flightPlans.TryRemove(id, out _);
        }

        public string ProvideFlightId(FlightPlan flightPlan)
        {
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
