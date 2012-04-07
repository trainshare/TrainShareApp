using System;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.ViewModels;

namespace TrainShareApp {
    public class MainPageViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly ITwitterClient _twitterClient;
        private MainPage _view;

        public MainPageViewModel(
            INavigationService navigationService,
            ITwitterClient twitterClient)
        {
            _navigationService = navigationService;
            _twitterClient = twitterClient;
        }

        protected override void OnViewReady(object view)
        {
            _view = view as MainPage;
            base.OnViewReady(view);

            //var routes = new TimeTable();
            //routes.GetConnections("Geneva", "Lugano", DateTime.Now);

            //var twitter = new TwitterClient(Credentials.TwitterToken, Credentials.TwitterTokenSecret);
            //twitter.Login(_view.Browser);
        }

        public void SigninTwitter()
        {
            _navigationService
                .UriFor<LoginViewModel>()
                .WithParam(m => m.Client, "twitter")
                .Navigate();
        }
        public void SigninFacebook()
        {
            Console.WriteLine("Signin in Facebook");
        }
    }
}
