using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using RestSharp;
using RestSharp.Authenticators;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TwitterClient : ITwitterClient
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Globals _globals;

        public TwitterClient(Globals globals)
        {
            _globals = globals;
            _consumerKey = Credentials.TwitterToken;
            _consumerSecret = Credentials.TwitterTokenSecret;
        }

        #region ITwitterClient Members

        public Task Logout()
        {
            _globals.TwitterId = 0;
            _globals.TwitterName = string.Empty;
            _globals.TwitterToken = null;
            _globals.TwitterSecret = null;

            return TaskEx.Delay(0);
        }

        public bool IsLoggedIn
        {
            get { return _globals.TwitterId != 0; }
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

            _globals.TwitterId = int.Parse(accessToken["user_id"]);
            _globals.TwitterName = accessToken["screen_name"];
            _globals.TwitterToken = accessToken["oauth_token"];
            _globals.TwitterSecret = accessToken["oauth_token_secret"];

            return
                new TwitterToken
                    {
                        Id = _globals.TwitterId,
                        ScreenName = _globals.TwitterName,
                        AccessToken = _globals.TwitterToken,
                        AccessTokenSecret = _globals.TwitterSecret
                    };
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
                    .Where(e => Regex.IsMatch(e.EventArgs.Uri.ToString(), @"http://(www|m).bing.com"))
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
    }
}