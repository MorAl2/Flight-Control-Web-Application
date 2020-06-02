using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [JsonPropertyName("flight_id")]
        public string Flight_Id { get; set; }
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; } = -1;
        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }
        [JsonPropertyName("initial_location")]
        public LocationAndTime Initial_Location { get; set; }
        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; }

        // calculating the landing time.
        public DateTime GetLandingTime()
        {
            DateTime tempTime = this.Initial_Location.StartTime;
            foreach(Segment segment in Segments)
            {
                tempTime = tempTime.AddSeconds(segment.Timespan_seconds);
            }
            return tempTime;
        }

        // checking if the FlightPlan is Valid.
        public Boolean IsValidFlightPlan()
        {
            if (this.Company_Name != null && this.Initial_Location != null 
                && this.Passengers != -1 && this.Segments != null)
            {
                // checking if all the segments are good.
                bool res =  IsGoodSegments();
                if (this.Initial_Location.IsValidLocationAndTime() && res)
                {
                    return true;
                }
            }
            return false;
        }
        // checking if all the segments are good.
        public bool IsGoodSegments()
        {
            foreach (Segment segment in this.Segments)
            {
                if (!segment.IsValidSegment())
                {
                    return false;
                }
            }
            return true;
        }

        // getting the current location by Interpolation.
        public LocationAndTime GetCurrentLocation(DateTime rel)
        {
            //LocationAndTime start = new LocationAndTime();
            int segNum = 0;
            DateTime dateTimeTemp = this.Initial_Location.StartTime;
            // finding the current segment.
            for (; segNum < this.Segments.Count; segNum++)
            {
                dateTimeTemp = dateTimeTemp.AddSeconds(Segments[segNum].Timespan_seconds);
                if(rel.CompareTo(dateTimeTemp) <= 0)
                {
                    break;
                }
            }
            // if it wasnt found.
            if(segNum == this.Segments.Count)
            {
                return new LocationAndTime(0, 0, rel);
            }
            // calculating hte longtitude and latitude.
            if(segNum > 0)
            {
                TimeSpan timeSinceSegmentStarted = dateTimeTemp.Subtract(rel);
                timeSinceSegmentStarted = new TimeSpan(0, 0, 
                    (int)Segments[segNum].Timespan_seconds).Subtract(timeSinceSegmentStarted);
                double seconds = timeSinceSegmentStarted.TotalSeconds;
                double precentPassed = (seconds / Segments[segNum].Timespan_seconds) * 100;
                double x = Segments[segNum - 1].Latitude + (Segments[segNum].Latitude - 
                    Segments[segNum - 1].Latitude) * (precentPassed / 100);
                double y = Segments[segNum - 1].Longitude + (Segments[segNum].Longitude - 
                    Segments[segNum - 1].Longitude) * (precentPassed / 100);
                return new LocationAndTime(y, x, rel);
            }
            else if(segNum == 0)
            {
                TimeSpan timeSinceSegmentStarted = dateTimeTemp.Subtract(rel);
                timeSinceSegmentStarted = new TimeSpan(0, 0, 
                    (int)Segments[segNum].Timespan_seconds).Subtract(timeSinceSegmentStarted);
                double seconds = timeSinceSegmentStarted.TotalSeconds;
                double precentPassed = (seconds / Segments[segNum].Timespan_seconds) * 100;
                double x = this.Initial_Location.Latitude + (Segments[segNum].Latitude - 
                    this.Initial_Location.Latitude) * (precentPassed / 100);
                double y = this.Initial_Location.Longitude + (Segments[segNum].Longitude - 
                    this.Initial_Location.Longitude) * (precentPassed / 100);
                return new LocationAndTime(y, x, rel);
            }
            else
            {
                return new LocationAndTime(0, 0, DateTime.Now);
            }
        }
    }
}
