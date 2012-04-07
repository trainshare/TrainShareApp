
using System;
using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class LoginViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;
        private LoginView _view;

        public LoginViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public LoginViewModel(INavigationService navigationService, ITwitterClient twitterClient)
        {
            _navigationService = navigationService;
            _twitterClient = twitterClient;
        }

        public string Client { get; set; }

        protected override void OnViewReady(object view)
        {
            _view = view as LoginView;
            base.OnViewReady(view);

            switch (Client)
            {
                case "twitter":
                    _twitterClient
                        .Login(_view.Browser)
                        .Subscribe(token => _navigationService.UriFor<MainViewModel>().Navigate());
                    break;
                case "facebook":
                    break;
                default:
                    _navigationService.GoBack();
                    break;
            }
        }
    }
}
