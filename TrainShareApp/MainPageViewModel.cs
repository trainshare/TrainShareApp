using System.Diagnostics;
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

        protected override void OnActivate()
        {
            base.OnActivate();

            if (_globals.TrainshareId == 0)
            {
                NeedsLogin = true;
            }
            else
            {
                NeedsLogin = false;
                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();
            }
        }

        public void Login()
        {
            _navigationService
                .UriFor<AccountsViewModel>()
                .Navigate();
        }
    }
}