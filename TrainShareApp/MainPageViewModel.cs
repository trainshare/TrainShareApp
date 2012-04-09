using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Windows.Navigation;
using Caliburn.Micro;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;

namespace TrainShareApp
{
    public class MainPageViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly Globals _globals;
        private bool _needsLogin;
        private IDisposable _removeSubscription;

        public MainPageViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");

            NeedsLogin = true;
        }

        public MainPageViewModel(
            INavigationService navigationService,
            Globals globals)
        {
            _navigationService = navigationService;
            _globals = globals;
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
            NeedsLogin = string.IsNullOrEmpty(_globals.TrainshareId);

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
    }
}