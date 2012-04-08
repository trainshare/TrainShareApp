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
    public class TrainshareClient : ITrainshareClient
    {
        public Task<string> SendAccessToken(string network, string token, string tokenSecret)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("login", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new TrainshareCredentials
                            {
                                network = "twitter",
                                access_token = "krassesToken",
                                access_token_secret = "krassesTokenSecret"
                            });

            return
                client
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content))
                    .Select(json => json["trainsharingID"].Value<string>())
                    .ToTask();
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
                            network = "twitter",
                            access_token = "krassesToken",
                            access_token_secret = "krassesTokenSecret"
                        });

            return
                client
                    .ExecuteObservable<List<TrainshareFriend>>(request)
                    .SelectMany(response => response.Data);
        }

        public Task Checkin(int trainshareId, Connection connection)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new
                        {
                            TrainshareId = trainshareId,
                            connection.From.Departure,
                            connection.To.Arrival
                        });

            return
                client
                    .ExecuteObservable(request)
                    .ToTask();
        }
    }
}
