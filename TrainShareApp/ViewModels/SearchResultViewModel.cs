using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Telerik.Windows.Controls;
using TrainShareApp.Data;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class SearchResultViewModel : Screen
    {
        private readonly ILog _logger;
        private readonly INavigationService _navigationService;
        private readonly ITimeTable _timeTable;

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

            Results =
                checkpoints
                    .SelectMany(
                        from =>
                        checkpoints.Select(to => new Connection {From = from, To = to}));

            Loading = true;

            From = "Lausanne";
            To = "Genf";
            Via = string.Empty;
            IsArrival = true;
            Time = DateTime.Now;
        }

        public SearchResultViewModel(
            ILog logger,
            INavigationService navigationService,
            ITimeTable timeTable)
        {
            _logger = logger;
            _navigationService = navigationService;
            _timeTable = timeTable;
        }

        public bool Loading { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Via { get; set; }

        public bool IsArrival { get; set; }
        public bool IsDeparture { get { return !IsArrival; } }

        public bool IsTomorrow { get { return Time.TimeOfDay < DateTime.Now.TimeOfDay && Time > DateTime.Now; } }

        public DateTime Time { get; set; }

        public IEnumerable<Connection> Results { get; set; }

        protected override void OnActivate()
        {
            SubmitSearch();

            base.OnActivate();
        }

        public void ConnectionSelected(RadDataBoundListBox list)
        {
            var global = App.Instance;
            var connection = list.SelectedValue as Connection;

            if (connection == null || global == null) return;

            global.SearchSelection = connection;

            _navigationService
                .UriFor<CheckinViewModel>()
                .Navigate();
        }

        private async void SubmitSearch()
        {
            Results = null;
            Loading = true;

            if (Time < DateTime.Now.Subtract(App.SearchTimeTolerance))
                Time = Time.AddDays(1);

            try
            {
                var result = await _timeTable.GetConnections(From, To, Time, IsArrival);

                From = result.From.Name;
                To = result.To.Name;
                Results = result.Connections;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                RadMessageBox.Show("Sorry, there was an unexpected error for your request. Please try again later.");
                _navigationService.GoBack();
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
