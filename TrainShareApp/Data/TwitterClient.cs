using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.Phone.Controls;
using RestSharp;
using RestSharp.Authenticators;

namespace TrainShareApp.Data
{
    class TwitterClient : ITwitterClient
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly RestClient _client;

        private WebBrowser _browser;
        private IDisposable _subscription;

        private string _authToken;
        private string _authTokenSecret;

        private string _accessToken;
        private string _accessTokenSecret;

        private string _verifyer;


        public TwitterClient(string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;

            _client = new RestClient("https://api.twitter.com")
            {
                Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret)
            };
        }

        public bool HasToken
        {
            get { return !string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_authTokenSecret); }
        }

        public string AccessToken
        {
            get { return _accessToken; }
        }

        public string AccesTokenSecret
        {
            get { return _accessTokenSecret; }
        }

        public void Login(WebBrowser viewBrowser)
        {
            DisposeSubscription();

            _subscription =
                Observable
                    .FromEventPattern<NavigatingEventArgs>(
                        h => viewBrowser.Navigating += h,
                        h => viewBrowser.Navigating -= h)
                    .Subscribe(OnNavigating);
            _browser = viewBrowser;

            RequestAuth();
        }

        private void RequestAuth()
        {
            _client.ExecuteAsync(
                new RestRequest("oauth/request_token"),
                restResponse =>
                    {
                        var queryString = ParseQueryString(restResponse.Content);

                        _authToken = queryString["oauth_token"];
                        _authTokenSecret = queryString["oauth_token_secret"];

                        if (!string.IsNullOrEmpty(_authToken) && !string.IsNullOrEmpty(_authTokenSecret))
                        {
                            RequestVerifyer();
                        }
                    }
                );
        }

        private void RequestVerifyer()
        {
            var rq =
                new RestRequest("oauth/authorize")
                    .AddParameter("oauth_token", _authToken);

            _browser.Navigate(_client.BuildUri(rq));
        }

        private void RequestAccess()
        {
            _client.Authenticator = OAuth1Authenticator.ForAccessToken(_consumerKey, _consumerSecret, _authToken,
                                                                       _authTokenSecret, _verifyer);
            _client.ExecuteAsync(
                new RestRequest("oauth/access_token"),
                restResponse =>
                    {
                        var queryString = ParseQueryString(restResponse.Content);

                        _accessToken = queryString["oauth_token"];
                        _accessTokenSecret = queryString["oauth_token_secret"];
                    }
                );
        }

        private void OnNavigating(EventPattern<NavigatingEventArgs> e)
        {
            var address = e.EventArgs.Uri.ToString();

            if (Regex.IsMatch(address, @"http://(www|m).bing.com"))
            {
                var queryString = ParseQueryString(address.Substring(address.IndexOf('?') + 1));

                _verifyer = queryString["oauth_verifier"];
                if (queryString["oauth_token"] != _authToken)
                {
                    // Bad things happened!!!
                    throw new SecurityException("The oauth token did not match!");
                }

                DisposeSubscription();

                e.EventArgs.Cancel = true;

                RequestAccess();
            }
        }

        private void DisposeSubscription()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var decoded = HttpUtility.UrlDecode(queryString);

            return
                Regex
                    .Split(decoded, "&")
                    .Select(vp => Regex.Split(vp, "="))
                    .ToDictionary(singlePair => singlePair[0],
                                  singlePair => singlePair.Length == 2 ? singlePair[1] : string.Empty);
        }
    }
}