using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {   
        [JsonPropertyName("longitude")]
        public Double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; }
        [JsonPropertyName("timespan_seconds")]
        public Double Timespan_seconds { get; set; }

        public override string ToString()
        {
            return "My OSet is" + Longitude;
        }
    }
}
