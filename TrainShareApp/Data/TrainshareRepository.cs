using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using RestSharp;
using TrainShareApp.Model;
using TrainShareApp.Extension;

namespace TrainShareApp.Data
{
    public class TrainshareRepository : ITrainshareRepository
    {
        public IObservable<string> SendAccessToken(string network, string token, string tokenSecret)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("login", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new TrainshareCredentials {
                                Network = "twitter",
                                AccessToken = "krassesToken",
                                AccessTokenSecret = "krassesTokenSecret"
                            });

            return
                client
                    .ExecuteObservable(request)
                    .Select(response => response.Content);
        }

        public IObservable<TrainshareFriend> GetFriends()
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new TrainshareCredentials
                            {
                                Network = "twitter",
                                AccessToken = "krassesToken",
                                AccessTokenSecret = "krassesTokenSecret"
                            });

            return
                client
                    .ExecuteObservable<List<TrainshareFriend>>(request)
                    .SelectMany(response => response.Data);
        }

        public IObservable<Unit> Checkin(string trainshareId, Connection connection)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new
                        {
                            TrainshareId = trainshareId,
                            Departure = connection.From.Departure,
                            Arrival = connection.To.Arrival
                        });

            return
                client
                    .ExecuteObservable(request)
                    .Select(response => new Unit());
        }
    }
}
