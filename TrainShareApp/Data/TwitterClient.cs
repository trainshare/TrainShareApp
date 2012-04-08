using System;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using RestSharp;
using RestSharp.Authenticators;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TwitterClient : ITwitterClient
    {
        private readonly Globals _globals;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly OAuthToken _token;

        public TwitterClient(Globals globals)
        {
            _globals = globals;
            _consumerKey = Credentials.TwitterToken;
            _consumerSecret = Credentials.TwitterTokenSecret;
            _token = new OAuthToken();
        }

        public IObservable<OAuthToken> Login(WebBrowser browser)
        {
            var subject = new AsyncSubject<OAuthToken>();
            
            var client = new RestClient("https://api.twitter.com/oauth/");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

            client
                .ExecuteObservable(new RestRequest("request_token"))
                .Select(response => response.Content)
                .ParseQueryString()
                .Select( // Getting an request token
                queryString =>
                    {
                        var token = queryString["oauth_token"];
                        var tokenSecret = queryString["oauth_token_secret"];

                        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret))
                            throw new SecurityException("Did not get an request token from twitter.");

                        return _token.WithAuth(token, tokenSecret);
                    })
                .Subscribe(
                    token => Verify(browser, subject),
                    subject.OnError);

            return subject;
        }

        public IObservable<Unit> Logout()
        {
            var subject = new AsyncSubject<Unit>();

            _globals.TwitterToken = null;
            _globals.TwitterSecret = null;

            subject.OnCompleted();

            return subject;
        }

        public bool IsLoggedIn
        {
            get
            {
                return
                    !string.IsNullOrEmpty(_globals.TwitterToken) &&
                    !string.IsNullOrEmpty(_globals.TwitterSecret);
            }
        }

        private void Verify(WebBrowser browser, IObserver<OAuthToken> subject)
        {
            var client = new RestClient("https://api.twitter.com/oauth/");
            client.FollowRedirects = true;
            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

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
                .Select(queryString => _token.WithVerifier(queryString["oauth_verifier"]))
                .Subscribe(
                verifier => RequestAccess(subject),
                subject.OnError);

            var request =
                new RestRequest("authorize")
                    .AddParameter("oauth_token", _token.AuthToken);

            browser.Navigate(client.BuildUri(request));
        }

        private void RequestAccess(IObserver<OAuthToken> subject)
        {
            var client = new RestClient("https://api.twitter.com/oauth/");
            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                _consumerKey, _consumerSecret,
                _token.AuthToken, _token.AuthTokenSecret,
                _token.Verifier);

            client
                .ExecuteObservable(new RestRequest("access_token"))
                .Select(response => response.Content)
                .ParseQueryString()
                .Subscribe(
                    queryString =>
                        {
                            _token.WithAccess(queryString["oauth_token"],
                                              queryString["oauth_token_secret"]);

                            _globals.TwitterToken = _token.AccessToken;
                            _globals.TwitterSecret = _token.AccessTokenSecret;

                            Contract.Assert(!string.IsNullOrEmpty(_globals.TwitterToken));
                            Contract.Assert(!string.IsNullOrEmpty(_globals.TwitterSecret));

                            subject.OnNext(_token);
                            subject.OnCompleted();
                        },
                    subject.OnError,
                    subject.OnCompleted);
        }
    }
}