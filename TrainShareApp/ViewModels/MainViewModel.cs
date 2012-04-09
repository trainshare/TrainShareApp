using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private string _from = "Bern";
        private string _to = "Basel";
        private string _via = string.Empty;
        private DateTime _time = DateTime.Now;

        private readonly INavigationService _navigationService;

        private readonly IObservableCollection<TrainshareFriend> _friends =
            new BindableCollection<TrainshareFriend>();

        public MainViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            Friends.AddRange(Enumerable.Range(0, 10).Select(i => new TrainshareFriend{Name = "Friend #" + i}));
        }

        public MainViewModel(
            INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public IObservableCollection<TrainshareFriend> Friends
        {
            get { return _friends; }
        }

        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                NotifyOfPropertyChange(() => From);
            }
        }

        public string To
        {
            get { return _to; }
            set
            {
                _to = value;
                NotifyOfPropertyChange(() => To);
            }
        }

        public string Via
        {
            get { return _via; }
            set
            {
                _via = value;
                NotifyOfPropertyChange(() => Via);
            }
        }

        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        protected override void OnViewReady(object view)
        {
            //_friendsSubscription = _trainshareClient.GetFriends(_globals.TrainshareId).Subscribe(_friends.Add);
            base.OnViewReady(view);
        }

        public void SubmitSearch()
        {
            _navigationService
                .UriFor<SearchResultViewModel>()
                .WithParam(vm => vm.From, From)
                .WithParam(vm => vm.To, To)
                .WithParam(vm => vm.Time, Time)
                .Navigate();
        }
    }
}