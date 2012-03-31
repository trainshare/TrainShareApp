using Caliburn.Micro;

namespace TrainShareApp.ViewModels
{
    public class TrainSearchViewModel : Screen
    {
        private readonly INavigationService _navigationService;

        public TrainSearchViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}