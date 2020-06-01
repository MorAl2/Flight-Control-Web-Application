using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {

        [JsonPropertyName("flight_id")]
        public string Flight_id { get; set; } = "";

        [JsonPropertyName("longitude")]
        public Double Longitude { get; set; } = Double.NaN;

        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; } = Double.NaN;

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; } = -1;

        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; } = "";

        [JsonPropertyName("date_time")]
        public DateTime TakeOffTime { get; set; }

        [JsonPropertyName("is_external")]
        public Boolean Is_external { get; set; }
        public Flight()
        {

        }

        // creating flight based on FlightPlan.
        public Flight(FlightPlan flightPlan, bool is_external)
        {
            this.Flight_id = flightPlan.Flight_Id;
            // temp values.
            this.Longitude = 1;
            this.Latitude = 1;
            this.Passengers = flightPlan.Passengers;
            this.Company_Name = flightPlan.Company_Name;
            this.TakeOffTime = flightPlan.Initial_Location.StartTime;
            this.Is_external = is_external;
        }
        // checking if the flight is valid.
        public Boolean IsValidFlight()
        {
            if (this.Company_Name != "" && this.Longitude != Double.NaN &&
                this.Latitude != Double.NaN && this.Flight_id != "" 
                && this.Passengers != -1 && this.TakeOffTime != null)
            {
                if (this.Latitude >= -90 && this.Latitude <= 90 
                    && this.Longitude >= -180 && this.Longitude <= 180)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
