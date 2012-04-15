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
        public IObservable<Station> GetLocations(string locationName)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("stations")
                    .WithRootElement("stations")
                    .WithFormat(DataFormat.Json)
                    .AddParameter("query", locationName);

            return
                client
                    .ExecuteObservable<List<Station>>(request)
                    .SelectMany(response => response.Data);
        }

        public Task<SearchResult> GetConnections(string from, string to, DateTime departure)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("connections")
                    .AddParameter("from", from)
                    .AddParameter("to", to)
                    .AddParameter("date", departure.ToString("yyyy-MM-dd"))
                    .AddParameter("time", departure.ToString("HH:mm"));

            return
                client
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content).ToObject<SearchResult>())
                    .ToTask();
        }
    }
}
