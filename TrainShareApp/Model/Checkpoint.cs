using System;

namespace TrainShareApp.Model
{
    public class Checkpoint
    {
        public Location Station { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string Platform { get; set; }
        public Prognosis Prognosis { get; set; }
    }
}