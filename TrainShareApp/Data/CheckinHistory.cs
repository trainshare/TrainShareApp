using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CheckinHistory
    {
        [JsonProperty]
        public IEnumerable<Checkin> Checkins { get; private set; }

        public CheckinHistory()
        {
            Checkins = new List<Checkin>();
        }

        public void Add(Checkin checkin)
        {
            var values = Checkins as List<Checkin>;
            Debug.Assert(values != null);

            var existing = values.IndexOf(checkin);
            if (existing > -1)
            {
                values[existing] = checkin;
            }
            else
            {
                values.Add(checkin);
            }
        }

        public IEnumerable<Checkin> Get(int count = 10)
        {
            return Checkins.OrderByDescending(c => c.CheckinTime).Take(count);
        }

        public int Count { get { return Checkins.Count(); }}
    }
}