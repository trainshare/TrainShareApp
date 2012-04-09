using System;

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
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Get or Set the departure Station
        /// </summary>
        public string DepartureStation { get; set; }

        /// <summary>
        /// Get or Set the arrival time
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Get or Set the arrival station
        /// </summary>
        public string ArrivalStation { get; set; }
    }
}
