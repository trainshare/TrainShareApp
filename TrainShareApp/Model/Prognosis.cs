using System;
using Newtonsoft.Json;

namespace TrainShareApp.Model
{
    /*
     * {
     *     "platform": null,
     *     "arrival": null,
     *     "departure": null,
     *     "capacity1st": "1",
     *     "capacity2nd": "2"
     * }
     */

    public class Prognosis
    {
// ReSharper disable InconsistentNaming
        /// <summary>
        /// Get or Set the utilisation of the 1st class
        /// </summary>
        public string Capacity1st { get; set; }

        /// <summary>
        /// Get or Set the utilisatoin of the 2nd class
        /// </summary>
        public string Capacity2st { get; set; }
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Get or Set the platform
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Get or Set the date and time of arrival
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Get or Set the date and time of departure
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Departure { get; set; }
    }
}
