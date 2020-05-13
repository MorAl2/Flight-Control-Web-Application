using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public Flight()
        {

        }

        public Flight(FlightPlan flightPlan,bool is_external)
        {
            this.Flight_id = flightPlan.Flight_Id;
            // this.Longitude =  use Interploation
            // this.Latitude =  use Interploation
            this.Longitude = 1;
            this.Latitude = 1;
            this.Passengers = flightPlan.Passengers;
            this.Company_Name = flightPlan.Company_Name;
            this.TakeOffTime = flightPlan.Initial_Location.StartTime;
            this.Is_external = is_external;
        }


        [JsonPropertyName("flight_id")]
        public string Flight_id { get; set; }

        [JsonPropertyName("longitude")]
        public Double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; }

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime TakeOffTime { get; set; }

        [JsonPropertyName("is_external")]
        public Boolean Is_external { get; set; }
    }
}
