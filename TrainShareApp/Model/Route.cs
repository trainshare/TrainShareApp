﻿using System;

namespace TrainShareApp.Model
{
    public class Route
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
    }
}
