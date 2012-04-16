using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class AccountsViewModel : Screen,
        IHandle<FacebookToken>, IHandle<TwitterToken>,
        IHandle<Logout>,
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
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITrainshareClient trainshareClient,
            IFacebookClient facebookClient,
            ITwitterClient twitterClient)
        {
            _trainshareClient = trainshareClient;
            _facebookClient = facebookClient;
            _twitterClient = twitterClient;
            _navigationService = navigationService;
            _events = events;

            _events.Subscribe(this);
        }

        /// <summary>
        /// Get or Set the Twitter name of the logged in user
        /// </summary>
        public string TwitterName
        {
            get
            {
                return _twitterClient.IsLoggedIn
                           ? "Currently logged in as @" + _twitterClient.Token.ScreenName
                           : "Currently not logged in";
            }
        }

        /// <summary>
        /// Get the Facebook name of the logged in user
        /// </summary>
        public string FacebookName
        {
            get { return _facebookClient.IsLoggedIn
                           ? "Currently logged in as " + _facebookClient.Token.ScreenName
                           : "Currently not logged in"; }
        }

        public bool CanSave
        {
            get { return !string.IsNullOrEmpty(_trainshareClient.Token.Id); }
        }

        public bool CanConnectFb { get { return !_facebookClient.IsLoggedIn; } }
        public bool CanDisconnectFb { get { return _facebookClient.IsLoggedIn; } }

        public bool CanConnectTw { get { return !_twitterClient.IsLoggedIn; } }
        public bool CanDisconnectTw { get { return _twitterClient.IsLoggedIn; } }

        public void ConnectFb()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "facebook")
                .Navigate();
        }

        public void DisconnectFb()
        {
            _facebookClient.LogoutAsync();
        }

        public void ConnectTw()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "twitter")
                .Navigate();
        }

        public void DisconnectTw()
        {
                _twitterClient.LogoutAsync();
        }

        public void Save()
        {
            _navigationService
                .UriFor<MainViewModel>()
                .Navigate();
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

        public void Handle(TwitterToken message)
        {
            UpdateTwitter();
        }

        public void Handle(TrainshareToken message)
        {
            NotifyOfPropertyChange(() => CanSave);
        }


        public void Handle(Logout message)
        {
            switch (message)
            {
                case Logout.Facebook:
                    UpdateFacebook();
                    break;
                case Logout.Twitter:
                    UpdateTwitter();
                    break;
            }
        }

        private void UpdateFacebook()
        {
            NotifyOfPropertyChange(() => FacebookName);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanConnectFb);
            NotifyOfPropertyChange(() => CanDisconnectFb);
        }

        private void UpdateTwitter()
        {
            NotifyOfPropertyChange(() => TwitterName);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanConnectTw);
            NotifyOfPropertyChange(() => CanDisconnectTw);
        }
    }
}
