using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class AccountsViewModel : Screen,
        IHandle<FacebookToken>, IHandle<TwitterToken>,
        IHandle<LogoutFacebook>, IHandle<LogoutTwitter>,
        IHandle<TrainshareToken>
    {
        private readonly ITrainshareClient _trainshareClient;
        private readonly IFacebookClient _facebookClient;
        private readonly ITwitterClient _twitterClient;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _events;

        public AccountsViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public AccountsViewModel(
            ITrainshareClient trainshareClient,
            IFacebookClient facebookClient,
            ITwitterClient twitterClient,
            INavigationService navigationService,
            IEventAggregator events)
        {
            _trainshareClient = trainshareClient;
            _facebookClient = facebookClient;
            _twitterClient = twitterClient;
            _navigationService = navigationService;
            _events = events;
        }

        /// <summary>
        /// Get or Set the Twitter name of the logged in user
        /// </summary>
        public string TwitterName
        {
            get { return _twitterClient.IsLoggedIn ? _twitterClient.Token.ScreenName : string.Empty; }
        }

        /// <summary>
        /// Get or Set the text of the login/logout button
        /// </summary>
        public string TwitterText
        {
            get { return _twitterClient.IsLoggedIn ? "Logout" : "Login"; }
        }

        /// <summary>
        /// Get if the user is logged in to Twitter
        /// </summary>
        public bool TwitterLoggedIn
        {
            get { return _twitterClient.IsLoggedIn; }
        }

        /// <summary>
        /// Get the Facebook name of the logged in user
        /// </summary>
        public string FacebookName
        {
            get { return _facebookClient.IsLoggedIn ? _facebookClient.Token.ScreenName : string.Empty; }
        }

        /// <summary>
        /// Get the text of the login/logout button
        /// </summary>
        public string FacebookText
        {
            get { return _facebookClient.IsLoggedIn ? "Logout" : "Login"; }
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
            get { return !string.IsNullOrEmpty(_trainshareClient.Token.Id); }
        }

        public void TwitterButton()
        {
            if (TwitterLoggedIn)
            {
                _twitterClient.LogoutAsync();
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
            if (FacebookLoggedIn)
            {
                _facebookClient.LogoutAsync();
            }
            else
            {
                _navigationService
                    .UriFor<LoginViewModel>()
                    .WithParam(vm => vm.Client, "facebook")
                    .Navigate();
            }
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
            UpdateFacebook();
            UpdateTwitter();

            base.OnActivate();
        }

        public void Handle(FacebookToken message)
        {
            UpdateFacebook();
        }

        public void Handle(LogoutFacebook message)
        {
            UpdateFacebook();
        }

        public void Handle(TwitterToken message)
        {
            UpdateTwitter();
        }

        public void Handle(LogoutTwitter message)
        {
            UpdateTwitter();
        }

        public void Handle(TrainshareToken message)
        {
            NotifyOfPropertyChange(() => CanSave);
        }

        private void UpdateFacebook()
        {
            NotifyOfPropertyChange(() => FacebookName);
            NotifyOfPropertyChange(() => FacebookText);
            NotifyOfPropertyChange(() => CanSave);
        }

        private void UpdateTwitter()
        {
            NotifyOfPropertyChange(() => TwitterName);
            NotifyOfPropertyChange(() => TwitterText);
            NotifyOfPropertyChange(() => CanSave);
        }
    }
}
