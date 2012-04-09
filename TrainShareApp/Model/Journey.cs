namespace TrainShareApp.Model
{
    /*
     * {
     *     "name": "ICE 374 ",
     *     "category": "ICE",
     *     "number": "374",
     *     "operator": null,
     *     "to": null
     * }
     */

    public class Journey
    {
        /// <summary>
        /// Get or Set the train name (ICE 374)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Get or Set the category of the train (ICE)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Get or Set the train number (374)
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Get or Set the operator
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Get or Set the to, unknown what this is for
        /// </summary>
        public string To { get; set; }
    }
}
