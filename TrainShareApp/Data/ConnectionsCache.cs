using System.Collections.Generic;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class ConnectionsCache
    {
        private IList<IList<Connection>> _cache =
            new List<IList<Connection>>();
 
        public int Add(IList<Connection> connections)
        {
            _cache.Add(connections);

            return _cache.Count - 1;
        }

        public IList<Connection> Get(int id)
        {
            return _cache[id];
        }

        public void Remove(int id)
        {
            _cache[id] = null;
        }
    }
}
