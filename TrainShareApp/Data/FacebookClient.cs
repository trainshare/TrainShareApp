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
    public class FacebookClient : TokenClientBase, IFacebookClient
    {
        private const string Redirect = "https://www.facebook.com/connect/login_success.html";
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public FacebookClient(IEventAggregator events, Func<DbDataContext> contextFactory)
            : base("facebook", events, contextFactory)
        {
            _consumerKey = Credentials.FacebookToken;
            _consumerSecret = Credentials.FacebookTokenSecret;

            ReloadToken();

            if (Token != null)
                Events.Publish(Token);
        }

        public Task LogoutAsync()
        {
            return TaskEx.Run(
                () =>
                {
                    DeleteToken();
                    Events.Publish(Dismiss.Facebook);
                });
        }

        public async Task<Token> LoginAsync(WebBrowser browser)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient("https://www.facebook.com/dialog/oauth/");
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

            InsertOrUpdateToken(
                new Token
                {
                    Id = user["id"].Value<int>(),
                    Network = "facebook",
                    ScreenName = user["name"].Value<string>(),
                    AccessToken = jsonToken["access_token"],
                    Expires = DateTime.Now + TimeSpan.FromSeconds(int.Parse(jsonToken["expires_in"]))
                });

            Events.Publish(Token);

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
    }
}
