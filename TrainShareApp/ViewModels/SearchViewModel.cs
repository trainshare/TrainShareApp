using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Caliburn.Micro;
using Telerik.Windows.Controls;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class SearchViewModel : Screen
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITimeTable _timeTable;

        public SearchViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");

            From = "Lausanne";
            To = "Genf";
            Via = string.Empty;
            Time = DateTime.Now;
        }

        public SearchViewModel(
            ILog logger,
            IEventAggregator events,
            INavigationService navigationService,
            ITimeTable timeTable)
        {
            _logger = logger;
            _events = events;
            _navigationService = navigationService;
            _timeTable = timeTable;

            _events.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            View.From.InitSuggestionsProvider(CreateProvider());
            View.To.InitSuggestionsProvider(CreateProvider());
            View.Via.InitSuggestionsProvider(CreateProvider());
        }

        private IAutoCompleteProvider CreateProvider()
        {
            var provider =
                new WebServiceAutoCompleteProvider
                {
                    FilterKeyProvider = o => (o as Station).Name,
                    FilterKeyPath = "Name"
                };

            Observable
                .FromEventPattern(
                    h => provider.InputChanged += h,
                    h => provider.InputChanged -= h)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(eve => _timeTable.GetLocations(provider.InputString))
                .Switch()
                .ObserveOnDispatcher()
                .Subscribe(stations => provider.LoadSuggestions(stations.OrderByDescending(st => st.Score)));

            return provider;
        }

        public bool IsArrival { get; set; }

        public string ArrivalString
        {
            get { return IsArrival ? "Arr" : "Dep"; }
        }

        public string From { get; set; }
        public string To { get; set; }
        public string Via { get; set; }

        public DateTime Time { get; set; }

        public void Search()
        {
            _navigationService
                .UriFor<SearchResultViewModel>()
                .WithParam(vm => vm.From, From)
                .WithParam(vm => vm.Via, Via)
                .WithParam(vm => vm.To, To)
                .WithParam(vm => vm.IsArrival, IsArrival)
                .WithParam(vm => vm.Time, Time)
                .Navigate();
        }

        public void RefreshTime()
        {
            Time = DateTime.Now;
        }

        private SearchView View { get { return GetView() as SearchView; } }
    }
}
