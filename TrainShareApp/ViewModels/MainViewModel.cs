using System;
using System.Diagnostics;
using System.Linq;
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

        private readonly ILog _logger;
        private readonly Globals _globals;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;

        private readonly IObservableCollection<TrainshareFriend> _friends =
            new BindableCollection<TrainshareFriend>();

        private Connection _currentCheckin;

        public MainViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            Friends.AddRange(Enumerable.Range(0, 10).Select(i => new TrainshareFriend{Name = "Friend #" + i}));
        }

        public MainViewModel(
            ILog logger,
            Globals globals,
            INavigationService navigationService,
            ITrainshareClient trainshareClient)
        {
            _logger = logger;
            _globals = globals;
            _navigationService = navigationService;
            _trainshareClient = trainshareClient;
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

        public bool HasCheckedIn { get { return CurrentCheckin != null; } }

        public Connection CurrentCheckin
        {
            get { return _currentCheckin; }
            set
            {
                _currentCheckin = value;
                NotifyOfPropertyChange(() => CurrentCheckin);
                NotifyOfPropertyChange(() => HasCheckedIn);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        
            if (_globals.CheckinConnection != null)
            {
                CurrentCheckin = _globals.CheckinConnection;
            }
        }

        protected async override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            try
            {
                var friends = await _trainshareClient.GetFriends(_globals.TrainshareId);

                Friends.Clear();
                Friends.AddRange(friends);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
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

        public void Settings()
        {
            _navigationService
                .UriFor<AccountsViewModel>()
                .Navigate();
        }

    }
}