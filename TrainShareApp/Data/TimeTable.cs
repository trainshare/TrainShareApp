using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TimeTable : ITimeTable
    {
        public Task<IEnumerable<Station>> GetLocations(string locationName)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("locations")
                    .WithRootElement("stations")
                    .WithFormat(DataFormat.Json)
                    .AddParameter("query", locationName);

            return
                client
                    .ExecutTaskAsync<List<Station>>(request)
                    .ContinueWith(task => task.Result.Data as IEnumerable<Station>);
        }

        public Task<SearchResult> GetConnections(string from, string to, DateTime time, bool isArrival = false)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("connections")
                    .AddParameter("from", from)
                    .AddParameter("to", to)
                    .AddParameter("date", time.ToString("yyyy-MM-dd"))
                    .AddParameter("time", time.ToString("HH:mm"))
                    .AddParameter("isArrivalTime", isArrival ? 1 : 0);

            return
                client
                    .ExecutTaskAsync(request)
                    .ContinueWith(task => JObject.Parse(task.Result.Content).ToObject<SearchResult>());
        }
    }
}
