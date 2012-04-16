using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Navigation;
using Caliburn.Micro;
using System;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;

namespace TrainShareApp
{
    public class MainPageViewModel : Screen, IHandle<FacebookToken>, IHandle<TwitterToken>, IHandle<TrainshareToken>
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;
        private readonly IFacebookClient _facebookClient;
        private readonly ITwitterClient _twitterClient;

        public MainPageViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public MainPageViewModel(
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITrainshareClient trainshareClient,
            IFacebookClient facebookClient,
            ITwitterClient twitterClient)
        {
            _logger = logger;
            _events = events;
            _navigationService = navigationService;
            _trainshareClient = trainshareClient;
            _facebookClient = facebookClient;
            _twitterClient = twitterClient;

            _events.Subscribe(this);
        }

        public bool CanTwitter
        {
            get { return !_twitterClient.IsLoggedIn; }
        }

        public void Twitter()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "twitter")
                .Navigate();
        }


        public bool CanFacebook
        {
            get { return !_facebookClient.IsLoggedIn; }
        }


        public void Facebook()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "facebook")
                .Navigate();
        }

        public bool CanContinue
        {
            get { return !CannotContinue; }
        }

        public bool CannotContinue
        {
            get { return string.IsNullOrEmpty(_trainshareClient.Token.Id); }
        }

        public void Continue()
        {
            _navigationService
                .UriFor<MainViewModel>()
                .Navigate();
        }

        public void Handle(FacebookToken message)
        {
            NotifyOfPropertyChange(() => CanFacebook);
        }

        public void Handle(TwitterToken message)
        {
            NotifyOfPropertyChange(() => CanTwitter);
        }

        public void Handle(TrainshareToken message)
        {
            NotifyOfPropertyChange(() => CanContinue);
        }

        protected override void OnInitialize()
        {
            if (CanContinue)
            {
                Continue();
            }

            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            if (CanContinue)
            {
                Observable
                    .FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(
                        h => _navigationService.Navigated += h,
                        h => _navigationService.Navigated -= h)
                    .Where(e => e.EventArgs.NavigationMode != NavigationMode.Back)
                    .Take(1)
                    .Subscribe(
                        e =>
                        {
                            var service = e.Sender as NavigationService;
                            Debug.Assert(service != null);

                            service.RemoveBackEntry();
                        });
            }

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            // Do this to make the first login dialog disappear
            NotifyOfPropertyChange(() => CannotContinue);

            base.OnDeactivate(close);
        }
    }
}