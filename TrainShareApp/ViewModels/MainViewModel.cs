using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private string _from = "Bern";
        private string _to = "Basel";
        private string _via = string.Empty;
        private DateTime _time = DateTime.Now;
        private IDisposable _friendsSubscription;

        private readonly INavigationService _navigationService;
        private readonly ITrainshareRepository _trainshareRepository;
        private readonly ITimeTable _timeTable;
        private readonly Globals _globals;

        private readonly IObservableCollection<TrainshareFriend> _friends =
            new BindableCollection<TrainshareFriend>();

        public MainViewModel()
        {
            Contract.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            Friends.AddRange(Enumerable.Range(0, 10).Select(i => new TrainshareFriend{Name = "Friend #" + i}));
        }

        public MainViewModel(
            INavigationService navigationService,
            ITrainshareRepository trainshareRepository,
            ITimeTable timeTable,
            Globals globals)
        {
            _navigationService = navigationService;
            _trainshareRepository = trainshareRepository;
            _timeTable = timeTable;
            _globals = globals;
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
            _friendsSubscription = _trainshareRepository.GetFriends().Subscribe(_friends.Add);
            base.OnViewReady(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (_friendsSubscription != null)
                    _friendsSubscription.Dispose();
            }

            base.OnDeactivate(close);
        }

        public void SubmitSearch()
        {
            _timeTable
                .GetConnections(From, To, Time)
                .ToList()
                .Subscribe(
                    connections =>
                        {
                            _globals.SearchResults = connections;
                            _navigationService
                                .UriFor<SearchResultViewModel>()
                                .Navigate();
                        },
                    exception =>
                    MessageBox.Show("Sorry, there was an unexpected error for your request. Please try again later."));
        }
    }
}