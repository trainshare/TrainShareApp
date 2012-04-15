using System;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Views;
using ArgumentException = System.ArgumentException;

namespace TrainShareApp.ViewModels
{
    public class LoginViewModel : Screen
    {
        private readonly IFacebookClient _facebookClient;
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;
        private readonly ITwitterClient _twitterClient;

        public LoginViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public LoginViewModel(
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITwitterClient twitterClient,
            IFacebookClient facebookClient,
            ITrainshareClient trainshareClient)
        {
            _navigationService = navigationService;
            _twitterClient = twitterClient;
            _facebookClient = facebookClient;
            _trainshareClient = trainshareClient;
            _logger = logger;
            _events = events;
        }

        public string Client { get; set; }

        protected override async void OnViewReady(object view)
        {
            var castedView = view as LoginView;
            Debug.Assert(castedView != null);

            switch (Client)
            {
                case "twitter":
                    try
                    {
                        var twitterToken =
                            await _twitterClient.Login(castedView.Browser);

                        _trainshareClient.SendAccessToken(
                            "twitter",
                            twitterToken.AccessToken,
                            twitterToken.AccessTokenSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        MessageBox.Show("I am terribly sorry but your request could not be finished.");
                    }

                    _navigationService.GoBack();
                    break;
                case "facebook":
                    try
                    {
                        var facebookToken =
                            await _facebookClient.Login(castedView.Browser);
                        
                        _trainshareClient.SendAccessToken(
                            "facebook",
                            facebookToken.AccessToken,
                            null);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                        MessageBox.Show("I am terribly sorry but your request could not be finished.");
                    }

                    _navigationService.GoBack();
                    break;
                default:
                    throw new ArgumentException("The client to login can only be twitter or facebook but it was " +
                                                Client);
            }

            base.OnViewReady(view);
        }
    }
}