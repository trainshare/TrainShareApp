using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Windows.Navigation;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;

namespace TrainShareApp
{
    public class MainPageViewModel : Screen, IHandle<TrainshareToken>
    {
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;
        private bool _needsLogin;
        private IDisposable _removeSubscription;

        public MainPageViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");

            NeedsLogin = true;
        }

        public MainPageViewModel(INavigationService navigationService, ITrainshareClient trainshareClient)
        {
            _navigationService = navigationService;
            _trainshareClient = trainshareClient;
        }

        public bool NeedsLogin
        {
            get { return _needsLogin; }
            set
            {
                _needsLogin = value;
                NotifyOfPropertyChange(() => NeedsLogin);
            }
        }

        protected override void OnInitialize()
        {
            NeedsLogin = string.IsNullOrEmpty(_trainshareClient.Token.Id);

            if (!NeedsLogin)
            {
                _removeSubscription =
                    Observable
                        .FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(
                            h => _navigationService.Navigated += h,
                            h => _navigationService.Navigated -= h)
                        .Subscribe(HandleNavigation);
            }

            base.OnInitialize();
        }

        protected override void OnViewReady(object view)
        {
            if (!NeedsLogin)
                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();

            base.OnViewReady(view);
        }

        public void Login()
        {
            _navigationService
                .UriFor<AccountsViewModel>()
                .Navigate();
        }

        private void HandleNavigation(EventPattern<NavigationEventArgs> e)
        {
            var source = e.Sender as NavigationService;

            if (_removeSubscription != null &&
                source != null &&
                source.Source.OriginalString.Contains("MainView.xaml"))
            {
                _removeSubscription.Dispose();
                _navigationService.RemoveBackEntry();
            }
        }

        public void Handle(TrainshareToken message)
        {
            NeedsLogin = false;
        }
    }
}