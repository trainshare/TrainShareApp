using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using RestSharp;
using RestSharp.Authenticators;
using TrainShareApp.Event;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TwitterClient : ITwitterClient, IHandle<Republish>
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly IEventAggregator _events;

        public TwitterClient(TwitterToken token, IEventAggregator events)
        {
            _events = events;
            _consumerKey = Credentials.TwitterToken;
            _consumerSecret = Credentials.TwitterTokenSecret;

            Token = token;

            _events.Subscribe(this);
        }

        #region ITwitterClient Members

        public TwitterToken Token { get; private set; }

        public Task LogoutAsync()
        {
            return TaskEx.Run(Logout);
        }

        public bool IsLoggedIn
        {
            get { return Token.Id != 0; }
        }

        public async Task<TwitterToken> Login(WebBrowser browser)
        {
            var client = new RestClient("https://api.twitter.com/oauth/");
            client.FollowRedirects = true;
            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

            var requestToken = await GetRequestToken(client);

            var uri = client.BuildUri(
                new RestRequest("authorize")
                    .AddParameter("oauth_token", requestToken["oauth_token"]));
            var verifier = await GetVerifier(browser, uri);

            client.Authenticator =
                OAuth1Authenticator.ForAccessToken(
                    _consumerKey, _consumerSecret,
                    requestToken["oauth_token"], requestToken["oauth_token_secret"],
                    verifier["oauth_verifier"]);
            var accessToken = await GetAccessToken(client);

            Debug.Assert(requestToken["oauth_token"] == verifier["oauth_token"]);

            Token.Id = int.Parse(accessToken["user_id"]);
            Token.ScreenName = accessToken["screen_name"];
            Token.AccessToken = accessToken["oauth_token"];
            Token.AccessTokenSecret = accessToken["oauth_token_secret"];

            _events.Publish(Token);

            return Token;
        }

        #endregion

        private async Task<IDictionary<string, string>> GetRequestToken(IRestClient client)
        {
            return await
                   client
                       .ExecuteObservable(new RestRequest("request_token"))
                       .Select(response => response.Content)
                       .ParseQueryString()
                       .ToTask();
        }

        private async Task<IDictionary<string, string>> GetVerifier(WebBrowser browser, Uri uri)
        {
            var task =
                Observable
                    .FromEventPattern<NavigatingEventArgs>(
                        h => browser.Navigating += h,
                        h => browser.Navigating -= h)
                    .Where(e => Regex.IsMatch(e.EventArgs.Uri.ToString(), @"http://trainshare.ch"))
                    .Take(1)
                    .Do(e => e.EventArgs.Cancel = true)
                    .Select(e => e.EventArgs.Uri.ToString())
                    .Select(address => address.Substring(address.IndexOf('?') + 1))
                    .ParseQueryString()
                    .ToTask();

            browser.Navigate(uri);

            return await task;
        }

        private async Task<IDictionary<string, string>> GetAccessToken(IRestClient client)
        {
            Debug.Assert(client.Authenticator is OAuth1Authenticator);

            return
                await
                client
                    .ExecuteObservable(new RestRequest("access_token"))
                    .Select(response => response.Content)
                    .ParseQueryString()
                    .ToTask();
        }

        private void Logout()
        {
            Token.Clear();
            _events.Publish(Event.Logout.Twitter);
        }

        public void Handle(Republish message)
        {
            if (message == Republish.TwitterToken)
            {
                _events.Publish(Token);
            }
        }
    }
}