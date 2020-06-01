using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class LocationAndTime
    {
        public LocationAndTime()
        {

        }

        public LocationAndTime(Double longi, Double lat, DateTime dateTime)
        {
            this.Latitude = lat;
            this.Longitude = longi;
            this.StartTime = dateTime;
        }
        [JsonPropertyName("longitude")]
        public Double Longitude { get; set; } = Double.NaN;
        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; } = Double.NaN;
        [JsonPropertyName("date_time")]
        public DateTime StartTime { get; set; }

        public bool IsValidLocationAndTime()
        {
            if (this.Latitude != Double.NaN && this.Longitude != Double.NaN && this.StartTime != null)
            {
                if (this.Latitude >= -90 && this.Latitude <= 90 && this.Longitude >= -180 && this.Longitude <= 180)
                {
                    return true;
                }
            }
            return false;
                
        }
    }
}
