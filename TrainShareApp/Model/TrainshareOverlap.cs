using System;
using Newtonsoft.Json;

namespace TrainShareApp.Model
{
    /*
     * {
     *     "departure_time":"2012-04-09T11:36:09+02:00",
     *     "departure_station":"Bern",
     *     "arrival_time":"2012-04-09T11:36:09+02:00",
     *     "arrival_station":"Basel SBB"
     * }
     */

    public class TrainshareOverlap
    {
        /// <summary>
        /// Get or Set the departure time
        /// </summary>
        [JsonProperty("departure_time")]
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Get or Set the departure Station
        /// </summary>
        [JsonProperty("departure_station")]
        public string DepartureStation { get; set; }

        /// <summary>
        /// Get or Set the arrival time
        /// </summary>
        [JsonProperty("arrival_time")]
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Get or Set the arrival station
        /// </summary>
        [JsonProperty("arrival_station")]
        public string ArrivalStation { get; set; }
    }
}
