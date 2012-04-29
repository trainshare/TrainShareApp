using System;
using System.Diagnostics;
using Caliburn.Micro;

namespace TrainShareApp.ViewModels
{
    public class SearchViewModel : Screen
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;

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
            INavigationService navigationService)
        {
            _logger = logger;
            _events = events;
            _navigationService = navigationService;

            From = "Lausanne";
            To = "Genf";
            Via = string.Empty;
            Time = DateTime.Now;

            _events.Subscribe(this);
        }

        public string From { get; set; }

        public string To { get; set; }

        public string Via { get; set; }

        public DateTime Time { get; set; }

        public void Continue()
        {
            _navigationService
                .UriFor<SearchResultViewModel>()
                .WithParam(vm => vm.From, From)
                .WithParam(vm => vm.Via, Via)
                .WithParam(vm => vm.To, To)
                .WithParam(vm => vm.DepartureTime, Time)
                .Navigate();
        }

        public void Cancel()
        {
            _navigationService.GoBack();
        }
    }
}
