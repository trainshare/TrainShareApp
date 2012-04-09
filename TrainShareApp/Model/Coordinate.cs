
namespace TrainShareApp.Model
{
    /*
     * {
     *     "type": "WGS84",
     *     "x": 7.439122,
     *     "y": 46.948825
     * }
     */

    public class Coordinate
    {
        /// <summary>
        /// Get or Set the type (WGS84)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Get or Set the x coordinate
        /// </summary>
        public string X { get; set; }

        /// <summary>
        /// Get or Set the y coordinate
        /// </summary>
        public string Y { get; set; }
    }
}
