using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.ViewModels
{
    public class HistoryControlViewModel : PropertyChangedBase
    {
        private readonly INavigationService _navigationService;


        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable Checkins { get; set; }
        public bool Expanded { get; set; }

        public HistoryControlViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void Toggle()
        {
            Expanded = !Expanded;
        }

        public void Expand()
        {
            Expanded = true;
        }

        public void Collapse()
        {
            Expanded = false;
        }

        public void TimeSelect(GestureEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;

            if (source == null) return;

            var grid = source.Parent<Grid>();

            if (grid == null) return;

            var checkin = grid.DataContext as Checkin;

            if (checkin == null) return;
            
            _navigationService
                .UriFor<SearchResultViewModel>()
                .WithParam(vm => vm.From, checkin.DepartureStation)
                .WithParam(vm => vm.To, checkin.ArrivalStation)
                .WithParam(vm => vm.IsArrival, false)
                .WithParam(vm => vm.Time, checkin.DepartureTime)
                .Navigate();
        }
    }
}