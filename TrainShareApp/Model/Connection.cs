using System;

namespace TrainShareApp.Model
{
    public class Connection
    {
        public DateTime Date { get; set; }
        public Checkpoint From { get; set; }
        public Checkpoint To { get; set; }
    }
}
