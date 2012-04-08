using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;

namespace TrainShareApp
{
    public class MainPageViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;
        private readonly IFacebookClient _facebookClient;
        private readonly Globals _globals;

        public MainPageViewModel(
            INavigationService navigationService,
            ITwitterClient twitterClient,
            IFacebookClient facebookClient,
            Globals globals)
        {
            _navigationService = navigationService;
            _twitterClient = twitterClient;
            _facebookClient = facebookClient;
            _globals = globals;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (string.IsNullOrEmpty(_globals.TrainshareId))
                _navigationService
                    .UriFor<AccountsViewModel>()
                    .WithParam(vm => vm.SkipBack, true)
                    .Navigate();
            else
                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();
        }

        public void SigninTwitter()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "twitter")
                .Navigate();
        }

        public void SigninFacebook()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(vm => vm.Client, "facebook")
                .Navigate();
        }
    }
}