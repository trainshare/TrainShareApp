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

        public bool CanSave
        {
            get { return !string.IsNullOrEmpty(_trainshareClient.Token.Id); }
        }

        public void Facebook()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "facebook");
        }

        public void Twitter ()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "twitter");
        }

        public void Save()
        {
            _navigationService.GoBack();
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
            NotifyOfPropertyChange(() => CanSave);
        }

        private void UpdateTwitter()
        {
            NotifyOfPropertyChange(() => CanSave);
        }
    }
}
