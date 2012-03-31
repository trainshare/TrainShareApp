using System;
using System.Text.RegularExpressions;
using System.Windows.Navigation;
using Caliburn.Micro;
using TrainShareApp.Data;

namespace TrainShareApp {
    public class MainPageViewModel : Screen
    {
        private MainPage _view;

        protected override void OnViewReady(object view)
        {
            _view = view as MainPage;
            base.OnViewReady(view);
        }
    }
}
