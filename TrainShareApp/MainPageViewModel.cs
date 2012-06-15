using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;
using System;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;
using TrainShareApp.Event;
using TrainShareApp.Views;

namespace TrainShareApp
{
    public class MainPageViewModel : Screen, IHandle<Token>, IHandle<Dismiss>
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

        public async void Twitter()
        {
            View.Browser.NavigateToString(string.Empty);
            View.Window.IsOpen = true;

            try
            {
                await _twitterClient.LoginAsync(View.Browser);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                View.Window.IsOpen = false;
            }
        }

        public bool CanFacebook
        {
            get { return !_facebookClient.IsLoggedIn; }
        }

        public async void Facebook()
        {
            View.Browser.NavigateToString(string.Empty);
            View.Window.IsOpen = true;

            try
            {
                await _facebookClient.LoginAsync(View.Browser);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                View.Window.IsOpen = false;
            }
        }

        private MainPage View
        {
            get { return GetView() as MainPage; }
        }

        public bool CanContinue
        {
            get { return _facebookClient.IsLoggedIn || _twitterClient.IsLoggedIn; }
        }

        public bool CannotContinue
        {
            get { return !CanContinue; }
        }

        public bool ShowLogin
        {
            get { return !_trainshareClient.IsLoggedIn; }
        }

        public async void Continue()
        {
            try
            {
                if (_facebookClient.IsLoggedIn)
                    await _trainshareClient.LoginAsync(_facebookClient.Token);

                if (_twitterClient.IsLoggedIn)
                    await _trainshareClient.LoginAsync(_twitterClient.Token);

                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                MessageBox.Show("I am terribly sorry but we could not authenticate with Trainshare.");
            }
        }

        public void Handle(Token message)
        {
            NotifyOfPropertyChange(() => CanFacebook);
            NotifyOfPropertyChange(() => CanTwitter);
            NotifyOfPropertyChange(() => CanContinue);
        }

        public void Handle(Dismiss message)
        {
            if (message == Dismiss.Facebook) NotifyOfPropertyChange(() => CanFacebook);
            if (message == Dismiss.Twitter) NotifyOfPropertyChange(() => CanTwitter);
        }

        protected override void OnInitialize()
        {
            if (_trainshareClient.IsLoggedIn)
            {
                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();
            }

            base.OnInitialize();
        }

        private IDisposable _removeSubscription;

        protected override void OnActivate()
        {
            if (CanContinue && _removeSubscription == null)
            {
                _removeSubscription =
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