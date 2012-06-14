using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class SearchResultViewModel : Screen, IHandle<Republish>
    {
        private readonly ILog _logger;
        private readonly INavigationService _navigationService;
        private readonly ITimeTable _timeTable;
        private readonly IEventAggregator _events;

        private readonly IObservableCollection<Connection> _results =
            new BindableCollection<Connection>();

        private Connection _selectedConnection;

        public SearchResultViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            var locations =
                Enumerable
                    .Range(0, 5)
                    .Select(i => new Station { Id = i.ToString(), Name = "Station " + i, Score = i.ToString(), Type = "TestType" })
                    .ToArray();
            var checkpoints =
                locations
                    .Select(
                        location =>
                        new Checkpoint
                        {
                            Arrival = DateTime.Now,
                            Departure = DateTime.Now,
                            Platform = location.Score,
                            Station = location
                        })
                    .ToArray();

            Results.AddRange(
                checkpoints
                    .SelectMany(
                        from =>
                        checkpoints.Select(to => new Connection {From = from, To = to})));

            Loading = true;

            From = "Lausanne";
            To = "Genf";
            Via = string.Empty;
            IsArrival = true;
            Time = DateTime.Now;
        }

        public SearchResultViewModel(
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITimeTable timeTable)
        {
            _logger = logger;
            _navigationService = navigationService;
            _timeTable = timeTable;
            _events = events;

            _events.Subscribe(this);
        }

        public string From { get; set; }
        public string To { get; set; }
        public string Via { get; set; }
        public bool IsArrival { get; set; }
        public bool IsDeparture { get { return !IsArrival; } }
        public DateTime Time { get; set; }

        public bool Loading { get; set; }

        public IObservableCollection<Connection> Results { get { return _results; } }

        public Connection SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                _selectedConnection = value;
                NotifyOfPropertyChange(() => SelectedConnection);

                // Handle selection
                SelectionChanged();
            }
        }

        public void Handle(Republish message)
        {
            if (message == Republish.Connection &&
                SelectedConnection != null)
            {
                _events.Publish(SelectedConnection);
            }
        }

        protected override void OnActivate()
        {
            SubmitSearch();

            base.OnActivate();
        }

        private void SelectionChanged()
        {
            if (SelectedConnection == null)
                return; // We did a manual reset

            _events.Publish(SelectedConnection);

            _navigationService
                .UriFor<CheckinViewModel>()
                .Navigate();
        }

        private async void SubmitSearch()
        {
            Results.Clear();
            Loading = true;

            try
            {
                var result = await _timeTable.GetConnections(From, To, Time);

                From = result.From.Name;

                To = result.To.Name;

                Loading = false;
                Results.AddRange(result.Connections);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                Loading = false;
                MessageBox.Show("Sorry, there was an unexpected error for your request. Please try again later.");
                _navigationService.GoBack();
            }
        }
    }
}
