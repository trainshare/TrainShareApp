namespace TrainShareApp.Model
{
    /*
     * {
     *      "journey": {
     *          ...
     *      },
     *      "departure": {
     *          ...
     *      },
     *      "arrival": {
     *          ...
     *      }
     *  }
     */

    public class Section
    {
        /// <summary>
        /// Get or Set general train information
        /// </summary>
        public Journey Journey { get; set; }

        /// <summary>
        /// Get or Set the departure time, station and platform
        /// </summary>
        public Checkpoint Departure { get; set; }

        /// <summary>
        /// Get or Set the arrival time, station and platform
        /// </summary>
        public Checkpoint Arrival { get; set; }
    }
}
