using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using Telerik.Windows.Controls;
using TrainShareApp.Data;
using TrainShareApp.Model;
using TrainShareApp.Event;

namespace TrainShareApp.ViewModels
{
    public class MainViewModel : Screen, IHandle<Checkin>, IHandle<Dismiss>
    {
        private readonly ILog _logger;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;

        public MainViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            Friends = Enumerable.Range(0, 10).Select(i => new TrainshareFriend { Name = "Friend #" + i }).ToList();

            CurrentCheckin =
                new Checkin
                    {
                        DepartureStation = "Maienfeld",
                        DepartureTime = DateTime.Now,
                        ArrivalStation = "Chur",
                        ArrivalTime = DateTime.Now,
                        Position = 0.5f,
                        CheckinTime = DateTime.Now.AddDays(1)
                    };
					
			History = Enumerable.Range(0, 5).Select(i => 
					new Checkin
					{
						DepartureStation = "Maienfeld",
						ArrivalStation = "Lausanne",
						DepartureTime = DateTime.Now.AddHours(i),
						ArrivalTime = DateTime.Now.AddHours(i + 1)
					}).ToList();
        }

        public MainViewModel(
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

            Handle(trainshareClient.CurrentCheckin);
        }

        public Checkin CurrentCheckin { get; set; }

        public bool HasNotCheckedIn { get { return CurrentCheckin == null || CurrentCheckin.ArrivalTime <= DateTime.Now; } }
        public bool HasCheckedIn { get { return !HasNotCheckedIn; } }

        public IList<Checkin> History { get; private set; } 

        public bool HasNoHistory { get { return History == null || History.Count == 0; } }
        public bool HasHistory { get { return !HasNoHistory; } }

        public IList<TrainshareFriend> Friends { get; private set; }

        public bool HasNoFriends { get { return Friends == null || Friends.Count == 0; } }
        public bool HasFriends { get { return !HasNoFriends; } }

        public void Checkin()
        {
            _navigationService
                .UriFor<SearchViewModel>()
                .Navigate();
        }

        public void Checkout()
        {
            _trainshareClient.Checkout();
        }

        public void Settings()
        {
            _navigationService
                .UriFor<AccountsViewModel>()
                .Navigate();
        }

        public void About()
        {
            _navigationService
                .UriFor<AboutViewModel>()
                .Navigate();
        }

        public void HistorySelected(RadDataBoundListBox list)
        {
            var checkin = list.SelectedValue as Checkin;

            if (checkin == null) return;

            _navigationService
                .UriFor<SearchViewModel>()
                .WithParam(vm => vm.From, checkin.DepartureStation)
                .WithParam(vm => vm.To, checkin.ArrivalStation)
                .WithParam(vm => vm.IsArrival, false)
                .WithParam(vm => vm.Time, DateTime.Now.Date.Add(checkin.DepartureTime.TimeOfDay))
                .Navigate();
        }

        public void Handle(Checkin message)
        {
            CurrentCheckin = message;

            UpdateHistory();
            UpdateFriends();
        }

        private async void UpdateHistory()
        {
            try
            {
                History = (await _trainshareClient.GetHistory(10)).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private async void UpdateFriends()
        {
            try
            {
                Friends = (await _trainshareClient.GetFriends()).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void Handle(Dismiss message)
        {
            if (Dismiss.Checkin == message)
            {
                CurrentCheckin = null;
                Friends.Clear();
            }
        }
    }
}