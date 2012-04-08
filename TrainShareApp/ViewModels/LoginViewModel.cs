using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class LoginViewModel : Screen
    {
        private readonly IFacebookClient _facebookClient;
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;

        public LoginViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public LoginViewModel(INavigationService navigationService, ITwitterClient twitterClient,
                              IFacebookClient facebookClient)
        {
            _navigationService = navigationService;
            _twitterClient = twitterClient;
            _facebookClient = facebookClient;
        }

        public string Client { get; set; }

        protected override void OnViewReady(object view)
        {
            var castedView = view as LoginView;
            Contract.Assume(castedView != null);

            base.OnViewReady(view);

            switch (Client)
            {
                case "twitter":
                    _twitterClient
                        .Login(castedView.Browser)
                        .Subscribe(token => _navigationService.GoBack());
                    break;
                case "facebook":
                    _facebookClient
                        .Login(castedView.Browser)
                        .Subscribe(token => _navigationService.GoBack());
                    break;
                default:
                    throw new ArgumentException("The client to login can only be twitter or facebook but it was " +
                                                Client);
            }
        }
    }
}