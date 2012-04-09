using System;
using Newtonsoft.Json;

namespace TrainShareApp.Model
{
    /*
     * {
     *      "station": {
     *          "id": "008507000",
     *          "name": "Bern",
     *          "score": null,
     *          "coordinate": {
     *              "type": "WGS84",
     *              "x": 7.439122,
     *              "y": 46.948825
     *          }
     *      },
     *      "arrival": null,
     *      "departure": "2012-04-09T11:04:00+0200",
     *      "platform": "7",
     *      "prognosis": {
     *          "platform": null,
     *          "arrival": null,
     *          "departure": null,
     *          "capacity1st": "1",
     *          "capacity2nd": "2"
     *      }
     *  }
     */

    public class Checkpoint
    {
        /// <summary>
        /// Get or Set station information like name, location and type
        /// </summary>
        public Station Station { get; set; }

        /// <summary>
        /// Get or Set the time of arrival
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Get or Set the time of departure
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Departure { get; set; }

        /// <summary>
        /// Get or Set the platform number
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Get or Set the prognosis about utilisation and search hit rating
        /// </summary>
        public Prognosis Prognosis { get; set; }
    }
}