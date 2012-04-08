using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class CheckinViewModel :Screen
    {
        private readonly Globals _globals;
        private readonly INavigationService _navigationService;
        private float _position = 0.5f;
        private string _message = string.Empty;
        private Connection _connection;

        public CheckinViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public CheckinViewModel(
            Globals globals,
            INavigationService navigationService)
        {
            _globals = globals;
            _navigationService = navigationService;
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

        protected override void OnActivate()
        {
            Connection = _globals.CheckinConnection;
            base.OnActivate();
        }

        public void Confirm()
        {
            
        }
    }
}
