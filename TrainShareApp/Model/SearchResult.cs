using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrainShareApp.Model
{
    public class SearchResult
    {
        public IList<Connection> Connections { get; set; }
        public Station From { get; set; }
        public Station To { get; set; }

        [JsonIgnore]
        public object Stations { get; set; }
    }
}
