using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class AccountsViewModel : Screen
    {
        private readonly Globals _globals;
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;
        private readonly IFacebookClient _facebookClient;

        private string _twitterName;
        private string _facebookName;
        private string _twitterText;
        private string _facebookText;

        public AccountsViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");

            TwitterName = "AdrianKuendig";
            FacebookName = "Adrian Kündig";
        }

        public AccountsViewModel(
            Globals globals,
            INavigationService navigationService,
            ITwitterClient twitterClient,
            IFacebookClient facebookClient)
        {
            _globals = globals;
            _navigationService = navigationService;
            _twitterClient = twitterClient;
            _facebookClient = facebookClient;
        }

        /// <summary>
        /// Set this to true if coming back from login to twitter or facebook
        /// </summary>
        public bool BackFromLogin { get; set; }

        /// <summary>
        /// Get or set if the view model is busy
        /// </summary>
        public bool Busy { get; set; }

        /// <summary>
        /// Get or Set the Twitter name of the logged in user
        /// </summary>
        public string TwitterName
        {
            get { return _twitterName; }
            set
            {
                _twitterName = value;
                NotifyOfPropertyChange(() => TwitterName);
            }
        }

        /// <summary>
        /// Get or Set the text of the login/logout button
        /// </summary>
        public string TwitterText
        {
            get { return _twitterText; }
            set
            {
                _twitterText = value;
                NotifyOfPropertyChange(() => TwitterText);
            }
        }

        /// <summary>
        /// Get if the user is logged in to Twitter
        /// </summary>
        public bool TwitterLoggedIn
        {
            get { return _twitterClient.IsLoggedIn; }
        }

        /// <summary>
        /// Get if the twitter button is enabled
        /// </summary>
        public bool CanTwitterButton
        {
            get { return !Busy; }
        }

        /// <summary>
        /// Get or Set the Facebook name of the logged in user
        /// </summary>
        public string FacebookName
        {
            get { return _facebookName; }
            set
            {
                _facebookName = value;
                NotifyOfPropertyChange(() => FacebookName);
            }
        }

        /// <summary>
        /// Get or Set the text of the login/logout button
        /// </summary>
        public string FacebookText
        {
            get { return _facebookText; }
            set
            {
                _facebookText = value;
                NotifyOfPropertyChange(() => FacebookText);
            }
        }

        /// <summary>
        /// Get if the user is logged in to Facebook
        /// </summary>
        public bool FacebookLoggedIn
        {
            get { return _globals.FacebookId != 0; }
        }

        /// <summary>
        /// Get if the facebook button is enabled
        /// </summary>
        public bool CanFacebookButton
        {
            get { return !Busy; }
        }

        public async void TwitterButton()
        {
            if (TwitterLoggedIn)
            {
                await _twitterClient.Logout();

                TwitterName = _globals.TwitterName;
                TwitterText = TwitterLoggedIn ? "Logout" : "Login";

            }
            else
            {
                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "twitter")
                    .Navigate();
            }
        }

        public async void FacebookButton()
        {
            if (TwitterLoggedIn)
            {
                await _facebookClient.Logout();

                FacebookName = _globals.FacebookName;
                FacebookText = FacebookLoggedIn ? "Logout" : "Login";
            }
            else
            {
                SetBusy(true);

                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "facebook")
                    .Navigate();
            }
        }

        protected override void OnActivate()
        {
            if (BackFromLogin)
            {
                BackFromLogin = false;
                SetBusy(false);
            }

            TwitterName = _globals.TwitterName;
            TwitterText = TwitterLoggedIn ? "Logout" : "Login";

            FacebookName = _globals.FacebookName;
            FacebookText = FacebookLoggedIn ? "Logout" : "Login";

            base.OnActivate();
        }

        private void SetBusy(bool enable)
        {
            if (enable)
            {
                Busy = true;
                NotifyOfPropertyChange(() => Busy);
                NotifyOfPropertyChange(() => CanTwitterButton);
                NotifyOfPropertyChange(() => CanFacebookButton);
            }
            else
            {
                Busy = false;
                NotifyOfPropertyChange(() => Busy);
                NotifyOfPropertyChange(() => CanTwitterButton);
                NotifyOfPropertyChange(() => CanFacebookButton);
            }
        }
    }
}
