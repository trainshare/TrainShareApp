using System;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class CheckinViewModel : Screen, IHandle<Connection>
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;

        public CheckinViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public CheckinViewModel(
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITrainshareClient trainshareClient)
        {
            _logger = logger;
            _events = events;
            _navigationService = navigationService;
            _trainshareClient = trainshareClient;

            _events.Subscribe(this);
        }

        public Checkin CurrentCheckin { get; set; }

        public float Position { get; set; }

        public string Message { get; set; }

        public async void Confirm()
        {
            try
            {
                // subtrack because the front is on the right
                var position = 10 - (int) Position*10;
                position = Math.Min(position, 10);
                position = Math.Max(position, 0);

                CurrentCheckin.Position = position;
                CurrentCheckin.CheckinTime = DateTime.Now;

                // need await to handle exception
                await _trainshareClient.Checkin(CurrentCheckin);
            }
            catch(Exception e)
            {
                _logger.Error(e);
                MessageBox.Show("Sorry, there was an unexpected error while checking in. Please try again later.");
            }

            _navigationService
                .UriFor<MainViewModel>()
                .Navigate();
        }

        public void Handle(Connection message)
        {
            CurrentCheckin = Checkin.FromConnection(message);
        }

        protected override void OnActivate()
        {
            _events.Publish(Republish.Connection);

            Position = 0.5f;

            base.OnActivate();
        }
    }
}
