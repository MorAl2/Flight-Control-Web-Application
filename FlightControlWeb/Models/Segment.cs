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
        public Double Longitude { get; set; } = Double.NaN;

        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; } = Double.NaN;

        [JsonPropertyName("timespan_seconds")]
        public Double Timespan_seconds { get; set; } = Double.NaN;

        // checking if the segment is valid.
        public Boolean IsValidSegment()
        {
            if(this.Latitude != Double.NaN && this.Longitude != Double.NaN && this.Timespan_seconds != Double.NaN)
            {
                if (this.Latitude >= -90 && this.Latitude <= 90 && this.Longitude >= -180 && this.Longitude <= 180 && Timespan_seconds >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
