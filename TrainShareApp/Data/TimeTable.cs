using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TimeTable : ITimeTable
    {
        public IObservable<IEnumerable<Station>> GetLocations(string locationName)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("locations")
                    .WithRootElement("stations")
                    .WithFormat(DataFormat.Json)
                    .AddParameter("query", locationName);

            return
                client
                    .ExecuteObservable<List<Station>>(request)
                    .Select(list => list.Data as IEnumerable<Station>);
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
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content).ToObject<SearchResult>())
                    .ToTask();
        }
    }
}
