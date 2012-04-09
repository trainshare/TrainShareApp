using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class SearchResultViewModel : Screen
    {
        private readonly Globals _globals;
        private readonly ILog _logger;
        private readonly INavigationService _navigationService;
        private readonly ITimeTable _timeTable;
        private IDisposable _selectionSubscription;

        private readonly IObservableCollection<Connection> _results =
            new BindableCollection<Connection>();

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
        }

        public SearchResultViewModel(
            INavigationService navigationService,
            ITimeTable timeTable,
            Globals globals,
            ILog logger)
        {
            _globals = globals;
            _logger = logger;
            _navigationService = navigationService;
            _timeTable = timeTable;
        }

        public string From { get; set; }
        public string To { get; set; }
        public string Via { get; set; }
        public DateTime Time { get; set; }

        public IObservableCollection<Connection> Results { get { return _results; } }

        protected override void OnActivate()
        {
            SubmitSearch();

            base.OnActivate();
        }

        protected override void OnViewReady(object view)
        {
            var castedView = view as SearchResultView;
            Debug.Assert(castedView != null);

            _selectionSubscription =
                Observable
                    .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                        t => castedView.Results.SelectionChanged += t,
                        t => castedView.Results.SelectionChanged -= t)
                    .Subscribe(SelectionChanged);

            base.OnViewReady(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (_selectionSubscription != null)
                _selectionSubscription.Dispose();

            base.OnDeactivate(close);
        }

        private void SelectionChanged(EventPattern<SelectionChangedEventArgs> e)
        {
            var listBox = e.Sender as ListBox;
            Debug.Assert(listBox != null);


            if (listBox.SelectedItem == null)
                return; // We did a manual reset

            _globals.CheckinConnection = listBox.SelectedItem as Connection;
            listBox.SelectedItem = null;

            _navigationService
                .UriFor<CheckinViewModel>()
                .Navigate();
        }

        private async void SubmitSearch()
        {
            Results.Clear();

            try
            {
                var result = await _timeTable.GetConnections(From, To, Time);

                From = result.From.Name;
                NotifyOfPropertyChange(() => From);

                To = result.To.Name;
                NotifyOfPropertyChange(() => To);

                Results.AddRange(result.Connections);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                MessageBox.Show("Sorry, there was an unexpected error for your request. Please try again later.");
                _navigationService.GoBack();
            }
        }
    }
}
