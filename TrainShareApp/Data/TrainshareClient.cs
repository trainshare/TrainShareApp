using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Extension;
using TrainShareApp.Model;
using TrainShareApp.Event;

namespace TrainShareApp.Data
{
    public class TrainshareClient : ITrainshareClient, IHandle<Republish>
    {
        private readonly CheckinHistory _history;
        private readonly IEventAggregator _events;

        public TrainshareClient(TrainshareToken token, Checkin currentCheckin, CheckinHistory history, IEventAggregator events)
        {
            _history = history;
            _events = events;

            Token = token;
            CurrentCheckin = currentCheckin;

            _events.Subscribe(this);
        }

        public TrainshareToken Token { get; private set; }

        public Checkin CurrentCheckin { get; private set; }

        public async Task<TrainshareToken> SendAccessToken(string network, string token, string tokenSecret)
        {
            var client = new RestClient("https://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("login", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddBody(
                        new JObject(
                            new JProperty("network", network),
                            new JProperty("access_token", token),
                            new JProperty("access_token_secret", tokenSecret)));

            var json =
                await
                client
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content))
                    .ToTask();

            Token.Id = json["trainshare_id"].Value<string>();
            Token.Token = json["trainshare_token"].Value<string>();

            _events.Publish(Token);

            return Token;
        }

        public async Task<IEnumerable<TrainshareFriend>> GetFriends()
        {
            var client = new RestClient("https://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("read", Method.GET)
                    .WithFormat(DataFormat.Json)
                    .AddParameter("trainshare_id", Token.Id);

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
        public async Task Checkin(Checkin checkin)
        {
            var client = new RestClient("https://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .WithFormat(DataFormat.Json)
                    .AddParameter("trainshare_id", Token.Id)
                    .AddParameter("trainshare_token", Token.Token)
                    .AddBody(
                        new JArray(
                            checkin
                                .Sections
                                .Select(
                                    section =>
                                    new JObject(
                                        new JProperty("departure_station", section.DepartureStation),
                                        new JProperty("departure_time", section.DepartureTime.ToString("HH:mm")),
                                        new JProperty("arrival_station", section.ArrivalStation),
                                        new JProperty("arrival_time", section.ArrivalTime.ToString("HH:mm")),
                                        new JProperty("train_id", section.TrainId),
                                        new JProperty("position", checkin.Position)))));

            //Skipping the result on purpouse
            await
                client
                    .ExecuteObservable(request)
                    .ToTask();

            _history.Add(checkin);
            CurrentCheckin.FromCheckin(checkin);

            _events.Publish(CurrentCheckin);
        }

        public void Handle(Republish message)
        {
            if (message == Republish.TrainshareToken && Token != null)
            {
                _events.Publish(Token);
            }
            else if (message == Republish.Checkin && CurrentCheckin != null)
            {
                _events.Publish(CurrentCheckin);
            }
        }
    }
}
