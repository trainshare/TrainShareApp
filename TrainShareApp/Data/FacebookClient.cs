using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json.Linq;
using RestSharp;
using TrainShareApp.Event;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class FacebookClient : IFacebookClient
    {
        private const string Redirect = "https://www.facebook.com/connect/login_success.html";
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Globals _globals;
        private readonly IEventAggregator _events;

        public FacebookClient(Globals globals, IEventAggregator events)
        {
            _globals = globals;
            _events = events;
            _consumerKey = Credentials.FacebookToken;
            _consumerSecret = Credentials.FacebookTokenSecret;
        }

        public Task LogoutAsync()
        {
            return TaskEx.Run(Logout);
        }

        private void Logout()
        {
            _globals.FacebookToken = null;
            _events.Publish(new LogoutFacebook());
        }

        public bool IsLoggedIn
        {
            get { return _globals.FacebookToken != null; }
        }

        public async Task<FacebookToken> Login(WebBrowser browser)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient("https://m.facebook.com/dialog/oauth/");
            var request =
                new RestRequest()
                    .AddParameter("client_id", _consumerKey)
                    .AddParameter("redirect_uri", Redirect)
                    .AddParameter("scope", string.Empty)
                    .AddParameter("state", guid)
                    .AddParameter("response_type", "token");

            browser.IsScriptEnabled = true;

            var uri = client.BuildUri(request);
            var jsonToken = await GetRequestToken(browser, uri);

            if (jsonToken.ContainsKey("error_reason"))
                return null;
            if (jsonToken["state"] != guid)
                throw new SecurityException("Cross site forgery happend, the state did not equal the guid");

            var user = await GetUserInfo(jsonToken["access_token"]);
            var token =
                new FacebookToken
                {
                    Id = user["id"].Value<int>(),
                    ScreenName = user["name"].Value<string>(),
                    AccessToken = jsonToken["access_token"],
                    Expires = DateTime.Now + TimeSpan.FromSeconds(int.Parse(jsonToken["expires_in"]))
                };

            _globals.FacebookToken = token;
            _events.Publish(token);

            return token;
        }

        private static async Task<IDictionary<string, string>> GetRequestToken(WebBrowser browser, Uri uri)
        {
            var task =
                Observable
                    .FromEventPattern<NavigatingEventArgs>(
                        h => browser.Navigating += h,
                        h => browser.Navigating -= h)
                    .Where(e => Regex.IsMatch(e.EventArgs.Uri.ToString(), '^' + Redirect))
                    .Take(1)
                    .Select(e => e.EventArgs.Uri.ToString())
                    .Select(address => address.Substring(address.IndexOf('#') + 1))
                    .ParseQueryString()
                    .ToTask();

            browser.Navigate(uri);

            return await task;
        }

        private async Task<JObject> GetUserInfo(string accessToken)
        {
            var client = new RestClient("https://graph.facebook.com/me");
            var request =
                new RestRequest()
                    .AddParameter("access_token", accessToken);

            return
                await
                client
                    .ExecuteObservable(request)
                    .Select(response => JObject.Parse(response.Content))
                    .ToTask();
        }
    }
}
