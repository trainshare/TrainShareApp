using System.Collections.Generic;

namespace TrainShareApp.Model
{
    public class Globals
    {
        public Connection CheckinConnection { get; set; }
        public IEnumerable<Connection> SearchResults { get; set; }

        public string TrainshareId { get; set; }

        public int TwitterId { get; set; }
        public string TwitterName { get; set; }
        public string TwitterToken { get; set; }
        public string TwitterSecret { get; set; }

        public int FacebookId { get; set; }
        public string FacebookName { get; set; }
        public string FacebookToken { get; set; }
    }
}