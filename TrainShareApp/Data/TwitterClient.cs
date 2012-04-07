using System;
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
        private readonly TwitterToken _token;

        public TwitterClient(Globals globals)
        {
            _globals = globals;
            _consumerKey = Credentials.TwitterToken;
            _consumerSecret = Credentials.TwitterTokenSecret;
            _token = new TwitterToken();
        }

        public IObservable<TwitterToken> Login(WebBrowser browser)
        {
            var subject = new AsyncSubject<TwitterToken>();
            
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

        private void Verify(WebBrowser browser, IObserver<TwitterToken> subject)
        {
            var client = new RestClient("https://api.twitter.com/oauth/");
            client.FollowRedirects = true;
            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

            Observable
                .FromEventPattern<NavigatingEventArgs>(
                    h => browser.Navigating += h,
                    h => browser.Navigating -= h)
                .Where(e => Regex.IsMatch(e.EventArgs.Uri.ToString(), @"http://(www|m).bing.com"))
                .Do(e => e.EventArgs.Cancel = true)
                .Take(1)
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

        private void RequestAccess(IObserver<TwitterToken> subject)
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

                            subject.OnNext(_token);
                            subject.OnCompleted();
                        },
                    subject.OnError,
                    subject.OnCompleted);
        }
    }
}