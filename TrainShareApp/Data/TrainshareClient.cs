using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Event;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TrainshareClient : TokenClientBase, ITrainshareClient
    {
        private readonly ILog _logger;

        public TrainshareClient(IEventAggregator events, Func<DbDataContext> contextFactory, ILog logger)
            : base("trainshare", events, contextFactory)
        {
            _logger = logger;
            ReloadToken();

            if (Token != null)
                Events.Publish(Token);

            ReloadCurrentCheckin();

            if(CurrentCheckin != null)
                Events.Publish(CurrentCheckin);
        }

        public Checkin CurrentCheckin { get; private set; }

        public async Task<Token> LoginAsync(Token token)
        {
            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return null;

            var client = new RestClient("http://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("login", Method.POST)
                    .AddJson(
                        new JObject(
                            new JProperty("network", token.Network),
                            new JProperty("access_token", token.AccessToken),
                            new JProperty("access_token_secret", token.AccessTokenSecret)));

            request.RequestFormat = DataFormat.Json;

            try
            {
                var json =
                    await
                    client
                        .ExecutTaskAsync(request)
                        .ContinueWith(task => JObject.Parse(task.Result.Content));

                InsertOrUpdateToken(
                    new Token
                    {
                        Network = "trainshare",
                        AccessToken = json["trainshare_id"].Value<string>(),
                        AccessTokenSecret = json["trainshare_token"].Value<string>()
                    });

                Events.Publish(Token);

                return Token;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public async Task<List<TrainshareFriend>> GetFriends()
        {
            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return new List<TrainshareFriend>();

            var client = new RestClient("http://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("read", Method.POST)
                    .AddParameter("trainshare_id", Token.AccessToken)
                    .AddParameter("trainshare_token", Token.AccessTokenSecret);

            try
            {
                return
                    await
                    client
                        .ExecutTaskAsync<List<TrainshareFriend>>(request)
                        .ContinueWith(task => task.Result.Data);
            }
            catch(Exception e)
            {
                _logger.Error(e);

                return new List<TrainshareFriend>();
            }
        }

        public Task<List<Checkin>> GetHistory(int count)
        {
            return TaskEx.Run(
                () =>
                {
                    using (var context = ContextFactory())
                    {
                        var checkins =
                            context
                                .Checkins
                                .OrderByDescending(c => c.CheckinTime)
                                .Take(count);

                        return checkins.ToList();
                    }
                });
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
            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return;

            var client = new RestClient("http://trainshare.herokuapp.com/v1/");
            var request =
                new RestRequest("checkin", Method.POST)
                    .AddJson(
                        new JObject(
                            new JProperty("trainshare_id", Token.AccessToken),
                            new JProperty("trainshare_token", Token.AccessTokenSecret),
                            new JProperty("data",
                                          new JArray(
                                              checkin
                                                  .Sections
                                                  .Select(
                                                      section =>
                                                      new JObject(
                                                          new JProperty("departure_station", section.DepartureStation),
                                                          new JProperty("departure_time",
                                                                        section.DepartureTime.ToString("HH:mm")),
                                                          new JProperty("arrival_station", section.ArrivalStation),
                                                          new JProperty("arrival_time",
                                                                        section.ArrivalTime.ToString("HH:mm")),
                                                          new JProperty("train_id", section.TrainId),
                                                          new JProperty("position", checkin.Position)))))));

            try
            {
                //Skipping the result on purpouse
                await client.ExecutTaskAsync(request);

                AddCheckin(checkin);
                CurrentCheckin = checkin;

                Events.Publish(CurrentCheckin);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public Task Checkout()
        {
            return TaskEx.Run(
                () =>
                {
                    if (CurrentCheckin == null)
                        return;

                    using (var context = ContextFactory())
                    {
                        context.ObjectTrackingEnabled = true;

                        context.Checkins.UpdateOnSubmit(c => c.Id == CurrentCheckin.Id, c => c.CheckedOut = true);

                        context.SubmitChanges();
                    }

                    CurrentCheckin = null;

                    Events.Publish(Dismiss.Checkin);
                });
        }

        private void ReloadCurrentCheckin()
        {
            using (var context = ContextFactory())
            {
                var checkin =
                    context
                        .Checkins
                        .OrderByDescending(c => c.CheckinTime)
                        .FirstOrDefault();

                if (checkin != null && !checkin.CheckedOut && checkin.ArrivalTime > DateTime.Now.Add(App.SearchTimeTolerance))
                    CurrentCheckin = checkin;
            }
        }

        private void AddCheckin(Checkin checkin)
        {
            using (var context = ContextFactory())
            {
                context.IncludeOne<Checkin>(t => t.Sections);
                context.ObjectTrackingEnabled = true;

                var old = context.Checkins.FirstOrDefault(
                    c =>
                    c.ArrivalStation.Equals(checkin.ArrivalStation) &&
                    c.ArrivalTime.Equals(checkin.ArrivalTime) &&
                    c.DepartureStation.Equals(checkin.DepartureStation) &&
                    c.DepartureTime.Equals(checkin.DepartureTime));

                if(old != null)
                {
                    context.CheckinSections.DeleteAllOnSubmit(old.Sections);
                    context.Checkins.DeleteOnSubmit(old);
                }

                context.Checkins.InsertOnSubmit(checkin);
                context.SubmitChanges();
            }
        }
    }
}
