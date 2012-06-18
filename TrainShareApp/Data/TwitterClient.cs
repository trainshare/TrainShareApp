using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
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
    public class TwitterClient : TokenClientBase, ITwitterClient
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public TwitterClient(IEventAggregator events, Func<DbDataContext> contextFactory)
            : base("twitter", events, contextFactory)
        {
            _consumerKey = Credentials.TwitterToken;
            _consumerSecret = Credentials.TwitterTokenSecret;

            ReloadToken();

            if (Token != null)
                Events.Publish(Token);
        }

        #region ITwitterClient Members

        public Task LogoutAsync()
        {
            return TaskEx.Run(
                () =>
                {
                    DeleteToken();
                    Events.Publish(Dismiss.Twitter);
                });
        }

        public async Task<Token> LoginAsync(WebBrowser browser)
        {
            // Check for network connectivity
            if (!NetworkInterface.GetIsNetworkAvailable()) return null;

            var client =
                new RestClient("https://api.twitter.com/oauth/")
                {
                    FollowRedirects = true,
                    Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret)
                };

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

            InsertOrUpdateToken(
                new Token
                {
                    Id = int.Parse(accessToken["user_id"]),
                    Network = "twitter",
                    ScreenName = accessToken["screen_name"],
                    AccessToken = accessToken["oauth_token"],
                    AccessTokenSecret = accessToken["oauth_token_secret"]
                });

            Events.Publish(Token);

            return Token;
        }

        #endregion

        private Task<IDictionary<string, string>> GetRequestToken(IRestClient client)
        {
            return
                client
                    .ExecutTaskAsync(new RestRequest("request_token"))
                    .ContinueWith(task => task.Result.Content)
                    .ParseQueryString();
        }

        private Task<IDictionary<string, string>> GetVerifier(WebBrowser browser, Uri uri)
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
                    .ToTask()
                    .ParseQueryString();

            browser.Navigate(uri);

            return task;
        }

        private Task<IDictionary<string, string>> GetAccessToken(IRestClient client)
        {
            Debug.Assert(client.Authenticator is OAuth1Authenticator);

            return
                client
                    .ExecutTaskAsync(new RestRequest("access_token"))
                    .ContinueWith(task => task.Result.Content)
                    .ParseQueryString();
        }
    }
}