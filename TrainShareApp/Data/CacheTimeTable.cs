using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class CacheTimeTable : ITimeTable
    {
        private readonly Dictionary<string, IList<Station>> _cache =
            new Dictionary<string, IList<Station>>();

        public async Task<IEnumerable<Station>> GetLocations(string locationName)
        {
            // First search in cache
            var cached = GetCachedLocation(locationName);
            if (cached != null) return cached as IEnumerable<Station>;

            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return Enumerable.Empty<Station>();

            // If nothing found in cache query sbb.ch
            var client = new RestClient("http://sbb-cache.herokuapp.com/");
            var request =
                new RestRequest("location")
                    .AddParameter("query", locationName, ParameterType.GetOrPost);

            try
            {
                var result =
                    await
                    client
                        .ExecutTaskAsync(request)
                        .ContinueWith(task =>
                                      {
                                          var res = JObject.Parse(task.Result.Content).Property("stations").Value;
                                          return res.ToObject<List<Station>>();
                                      });

                UpdateCachedLocation(locationName, result);

                return result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<Station>();
            }
        }

        public async Task<SearchResult> GetConnections(string from, string to, DateTime time, bool isArrival = false)
        {
            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return null;

            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("connections")
                    .AddParameter("from", from)
                    .AddParameter("to", to)
                    .AddParameter("date", time.ToString("yyyy-MM-dd"))
                    .AddParameter("time", time.ToString("HH:mm"))
                    .AddParameter("isArrivalTime", isArrival ? 1 : 0);

            try
            {
                return 
                    await
                    client
                        .ExecutTaskAsync(request)
                        .ContinueWith(task => JObject.Parse(task.Result.Content).ToObject<SearchResult>());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IList<Station> GetCachedLocation(string fragment)
        {
            lock (_cache)
            {
                IList<Station> result;

                _cache.TryGetValue(fragment, out result);

                return result;
            }
        }

        private void UpdateCachedLocation(string fragment, IList<Station> locations)
        {
            lock (_cache)
            {
                _cache[fragment] = locations;
            }
        }
    }
}