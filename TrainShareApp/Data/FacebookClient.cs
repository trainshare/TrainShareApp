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
        private readonly IEventAggregator _events;

        public FacebookClient(FacebookToken facebookToken, IEventAggregator events)
        {
            _events = events;
            _consumerKey = Credentials.FacebookToken;
            _consumerSecret = Credentials.FacebookTokenSecret;

            Token = facebookToken;
        }

        public FacebookToken Token { get; private set; }

        public Task LogoutAsync()
        {
            return TaskEx.Run(Logout);
        }

        public bool IsLoggedIn
        {
            get { return Token.Id != 0; }
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

            Token.Id = user["id"].Value<int>();
            Token.ScreenName = user["name"].Value<string>();
            Token.AccessToken = jsonToken["access_token"];
            Token.Expires = DateTime.Now + TimeSpan.FromSeconds(int.Parse(jsonToken["expires_in"]));

            _events.Publish(Token);

            return Token;
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

        private static async Task<JObject> GetUserInfo(string accessToken)
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

        private void Logout()
        {
            Token.Clear();
            _events.Publish(new LogoutFacebook());
        }
    }
}
