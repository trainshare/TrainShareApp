using System;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using Telerik.Windows.Controls;
using TrainShareApp.Data;
using TrainShareApp.Event;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class CheckinViewModel : Screen
    {
        private readonly ILog _logger;
        private readonly INavigationService _navigationService;
        private readonly ITrainshareClient _trainshareClient;

        public CheckinViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public CheckinViewModel(
            ILog logger,
            INavigationService navigationService,
            ITrainshareClient trainshareClient)
        {
            _logger = logger;
            _navigationService = navigationService;
            _trainshareClient = trainshareClient;
        }

        public Checkin CurrentCheckin { get; set; }

        public float Position { get; set; }

        public string Message { get; set; }

        public bool Loading { get; set; }

        public async void Confirm()
        {
            try
            {
                Loading = true;

                // subtrack because the front is on the right
                var position = 10 - (int) Position*10;
                position = Math.Min(position, 10);
                position = Math.Max(position, 0);

                CurrentCheckin.Position = position;
                CurrentCheckin.CheckinTime = DateTime.Now;

                // need await to handle exception
                await _trainshareClient.Checkin(CurrentCheckin);
            }
            catch(Exception e)
            {
                _logger.Error(e);
                RadMessageBox.Show("Sorry, there was an unexpected error while checking in. Please try again later.");
            }
            finally
            {
                Loading = false;

                _navigationService
                    .UriFor<MainViewModel>()
                    .Navigate();
            }
        }

        public void Cancel()
        {
            _navigationService.RemoveBackEntry();
            _navigationService.RemoveBackEntry();
            _navigationService.GoBack();
        }

        protected override void OnActivate()
        {
            Position = 0.5f;
            CurrentCheckin = Checkin.FromConnection(App.Instance.SearchSelection);

            base.OnActivate();
        }
    }
}
