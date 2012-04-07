using System.Collections.Generic;

namespace TrainShareApp.Model
{
    public class Globals
    {
        public Connection CheckinConnection { get; set; }
        public IEnumerable<Connection> SearchResults { get; set; }

        public string TrainshareId { get; set; }

        public string TwitterToken { get; set; }
        public string TwitterSecret { get; set; }

        public string FacebookToken { get; set; }
        public string FacebookSecret { get; set; }
    }
}