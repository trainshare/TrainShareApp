using System;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class CheckinViewModel : Screen, IHandle<Connection>
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;

        private float _position = .5f;
        private string _message = string.Empty;
        private Connection _connection;

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

        public Connection Connection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                NotifyOfPropertyChange(() => Connection);
            }
        }

        public float Position { 
            get { return _position; }
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public async void Confirm()
        {
            try
            {
                var position = (int)Position*10;
                position = Math.Min(position, 10);
                position = Math.Max(position, 0);

                await _trainshareClient.Checkin(Connection, position);
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
            Connection = message;
        }
    }
}
