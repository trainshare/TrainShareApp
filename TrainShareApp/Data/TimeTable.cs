using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using RestSharp;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TimeTable : ITimeTable
    {
        public IObservable<Location> GetLocations(string locationName)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("stations")
                    .WithRootElement("stations")
                    .WithFormat(DataFormat.Json)
                    .AddParameter("query", locationName);

            return
                client
                    .ExecuteObservable<List<Location>>(request)
                    .SelectMany(response => response.Data);
        }

        public IObservable<Connection> GetConnections(string from, string to, DateTime departure)
        {
            var client = new RestClient("http://transport.opendata.ch/v1/");
            var request =
                new RestRequest("connections")
                    .WithRootElement("connections")
                    .WithFormat(DataFormat.Json)
                    .AddParameter("from", from)
                    .AddParameter("to", to)
                    .AddParameter("date", departure.ToString("yyyy-MM-dd"))
                    .AddParameter("time", departure.ToString("HH:mm"));

            Debug.WriteLine(request.ToString());

            return
                client
                    .ExecuteObservable<List<Connection>>(request)
                    .SelectMany(response => response.Data);
        }
    }
}
