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
            this.latitude = lat;
            this.longitude = longi;
            this.StartTime = dateTime;
        }
        public Double longitude { get; set; } = Double.NaN;
        public Double latitude { get; set; } = Double.NaN;
        [JsonPropertyName("date_time")]
        public DateTime StartTime { get; set; }
    }
}
