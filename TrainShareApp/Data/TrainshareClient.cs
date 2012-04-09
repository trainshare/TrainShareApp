using System;
using System.Collections.Generic;
using System.Linq;
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
                        new JObject(
                            new JProperty("network", network),
                            new JProperty("access_token", token),
                            new JProperty("access_token_secret", tokenSecret)));

            return
                client
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content))
                    .Select(json => json["trainshare_id"].Value<string>())
                    .ToTask();
        }

        public async Task<IEnumerable<TrainshareFriend>> GetFriends(string trainshareId)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("read", Method.GET)
                    .WithFormat(DataFormat.Json)
                    .AddParameter("trainshare_id", trainshareId);

            return
                await
                client
                    .ExecuteObservable<List<TrainshareFriend>>(request)
                    .Select(response => response.Data)
                    .ToTask();
        }

        /*
         *     departure_station: "Bern",
         *     departure_time: "16:34",
         *     arrival_station: "Basel SBB",
         *     arrival_time: "17:29",
         *     train_id: "IC 1080"
         */
        public async Task Checkin(string trainshareId, Connection connection, int position)
        {
            var client = new RestClient("http://trainsharing.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddParameter("trainshare_id", trainshareId)
                    .AddBody(
                        new JArray(
                            connection
                                .Sections
                                .Select(
                                    section =>
                                    new JObject(
                                        new JProperty("departure_station", section.Departure.Station.Name),
                                        new JProperty("departure_time", section.Departure.Departure.ToString("HH:mm")),
                                        new JProperty("arrival_station", section.Arrival.Station.Name),
                                        new JProperty("arrival_time", section.Arrival.Arrival.ToString("HH:mm")),
                                        new JProperty("train_id", section.Journey.Name),
                                        new JProperty("position", position)))));


            //Skipping the result on purpouse
            await
                client
                    .ExecuteObservable(request)
                    .ToTask();
        }
    }
}
