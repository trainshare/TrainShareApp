
namespace TrainShareApp.Model
{
    /*
     * {
     *     "id": "008507000",
     *     "name": "Bern",
     *     "score": null,
     *     "coordinate": {
     *         "type": "WGS84",
     *         "x": 7.439122,
     *         "y": 46.948825
     *     }
     * }
     */

    public class Station
    {
        /// <summary>
        /// Get or Set the id (008507000)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Get or Set the type (unknown)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Get or Set the name (Bern)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or Set the score in search (101)
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// Get or Set the coordinates of the location
        /// </summary>
        public Coordinate Coordinates { get; set; }
    }
}
