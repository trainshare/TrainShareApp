using System.Diagnostics.Contracts;
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
            Contract.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");

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
        /// Get or Set if the last navigation frame should be skipped on back navigation
        /// </summary>
        public bool SkipBack { get; set; }

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
            get
            {
                return
                    !string.IsNullOrEmpty(_globals.TwitterToken) &&
                    !string.IsNullOrEmpty(_globals.TwitterSecret);
            }
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
            get
            {
                return
                    !string.IsNullOrEmpty(_globals.FacebookToken) &&
                    !string.IsNullOrEmpty(_globals.FacebookSecret);
            }
        }

        public void TwitterButton()
        {
            if (TwitterLoggedIn)
            {
                _twitterClient.Logout();
            }
            else
            {
                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "twitter")
                    .Navigate();
            }
        }

        public void FacebookButton()
        {
            if (TwitterLoggedIn)
            {
                _facebookClient.Logout();
            }
            else
            {
                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "facebook")
                    .Navigate();
            }
        }

        protected override void OnActivate()
        {
            TwitterText = TwitterLoggedIn ? "Logout" : "Login";
            FacebookText = FacebookLoggedIn ? "Logout" : "Login";

            base.OnActivate();
        }
    }
}
