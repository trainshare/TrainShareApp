using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class AccountsViewModel : Screen, IHandle<FacebookToken>, IHandle<TwitterToken>, IHandle<LogoutFacebook>, IHandle<LogoutTwitter>
    {
        private readonly Globals _globals;
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;
        private readonly IFacebookClient _facebookClient;
        private readonly IEventAggregator _events;

        private string _twitterName;
        private string _facebookName;
        private string _twitterText;
        private string _facebookText;
        private bool _canSave;

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
            IFacebookClient facebookClient,
            IEventAggregator events)
        {
            _globals = globals;
            _navigationService = navigationService;
            _twitterClient = twitterClient;
            _facebookClient = facebookClient;
            _events = events;
        }

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
            get { return _facebookClient.IsLoggedIn; }
        }

        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public async void TwitterButton()
        {
            if (TwitterLoggedIn)
            {
                await _twitterClient.LogoutAsync();
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
            if (FacebookLoggedIn)
            {
                await _facebookClient.LogoutAsync();
            }
            else
            {
                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "facebook")
                    .Navigate();
            }
        }

        private void UpdateFacebook(string name)
        {
            FacebookName = name;
            FacebookText = FacebookLoggedIn ? "Logout" : "Login";

            CanSave = !string.IsNullOrEmpty(_globals.TrainshareId);
        }

        private void UpdateTwitter(string name)
        {
            TwitterName = name;
            TwitterText = TwitterLoggedIn ? "Logout" : "Login";

            CanSave = !string.IsNullOrEmpty(_globals.TrainshareId);
        }

        public void Save()
        {
            _navigationService
                .UriFor<MainViewModel>()
                .Navigate();
        }

        protected override void OnInitialize()
        {
            _events.Subscribe(this);

            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            if (_globals.FacebookToken != null)
                UpdateFacebook(_globals.FacebookToken.ScreenName);
            else
                UpdateFacebook(string.Empty);

            if (_globals.TwitterToken != null)
                UpdateTwitter(_globals.TwitterToken.ScreenName);
            else
                UpdateTwitter(string.Empty);

            CanSave = !string.IsNullOrEmpty(_globals.TrainshareId);

            base.OnActivate();
        }

        public void Handle(FacebookToken message)
        {
            UpdateFacebook(message.ScreenName);
        }

        public void Handle(LogoutFacebook message)
        {
            UpdateFacebook(string.Empty);
        }

        public void Handle(TwitterToken message)
        {
            UpdateTwitter(message.ScreenName);
        }

        public void Handle(LogoutTwitter message)
        {
            UpdateTwitter(string.Empty);
        }
    }
}
