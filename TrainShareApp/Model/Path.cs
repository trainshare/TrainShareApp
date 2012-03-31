using System;
using System.Collections.Generic;

namespace TrainShareApp.Model
{
    public class Path
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
    }
}
