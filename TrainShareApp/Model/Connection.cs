using System;
using System.Collections.Generic;

namespace TrainShareApp.Model
{
    /*
     * {
     *  "from": {
     *      ...
     *  },
     *  "to": {
     *      ...
     *  },
     *  "sections": [{
     *      "journey": {
     *          ...
     *      },
     *      "departure": {
     *          ...
     *      },
     *      "arrival": {
     *          ...
     *      }
     *  }]
     */

    public class Connection
    {
        /// <summary>
        /// Get or Set the starting checkpoint
        /// </summary>
        public Checkpoint From { get; set; }

        /// <summary>
        /// Get or Set the end checkpoint
        /// </summary>
        public Checkpoint To { get; set; }

        /// <summary>
        /// Get or Set the sections in the route of travel
        /// </summary>
        public IList<Section> Sections { get; set; }
    }
}
