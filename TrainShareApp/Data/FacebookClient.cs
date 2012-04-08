using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class FacebookClient : IFacebookClient
    {
        private static readonly string Redirect = "https://www.facebook.com/connect/login_success.html";
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Globals _globals;

        public FacebookClient(Globals globals)
        {
            _globals = globals;
            _consumerKey = Credentials.FacebookToken;
            _consumerSecret = Credentials.FacebookTokenSecret;
        }

        public Task Logout()
        {
            _globals.FacebookId = 0;
            _globals.FacebookName = string.Empty;
            _globals.FacebookToken = null;

            return TaskEx.Delay(0);
        }

        public async Task<FacebookToken> Login(WebBrowser browser)
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
            var token = await GetRequestToken(browser, uri);

            if (token.ContainsKey("error_reason"))
                return null;
            if (token["state"] != guid)
                throw new SecurityException("Cross site forgery happend, the state did not equal the guid");

            var user = await GetUserInfo(token["access_token"]);

            _globals.FacebookId = user["id"].Value<int>();
            _globals.FacebookName = user["name"].Value<string>();
            _globals.FacebookToken = token["access_token"];

            return
                new FacebookToken
                {
                    Id = _globals.FacebookId,
                    ScreenName = _globals.FacebookName,
                    AccessToken = _globals.FacebookToken,
                };
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
