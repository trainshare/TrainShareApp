using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using TrainShareApp.Model;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class SearchResultViewModel : Screen
    {
        private readonly Globals _globals;
        private readonly INavigationService _navigationService;
        private IDisposable _selectionSubscription;

        private readonly IObservableCollection<Connection> _results =
            new BindableCollection<Connection>();

        public SearchResultViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
            var locations =
                Enumerable
                    .Range(0, 5)
                    .Select(i => new Location { Id = i.ToString(), Name = "Location " + i, Score = i.ToString(), Type = "TestType" })
                    .ToArray();
            var checkpoints =
                locations
                    .Select(
                        location =>
                        new Checkpoint
                        {
                            Arrival = DateTime.Now,
                            Departure = DateTime.Now,
                            Platform = location.Score,
                            Station = location
                        })
                    .ToArray();

            Results.AddRange(
                checkpoints
                    .SelectMany(
                        from =>
                        checkpoints.Select(to => new Connection {Date = DateTime.Now, From = from, To = to})));
        }

        public SearchResultViewModel(
            Globals globals,
            INavigationService navigationService)
        {
            _globals = globals;
            _navigationService = navigationService;
        }

        public IObservableCollection<Connection> Results { get { return _results; } }

        protected override void OnActivate()
        {
            Results.Clear();

            if (_globals.SearchResults != null)
                Results.AddRange(_globals.SearchResults);

            base.OnActivate();
        }

        protected override void OnViewReady(object view)
        {
            var castedView = view as SearchResultView;
            Contract.Assume(castedView != null);

            _selectionSubscription =
                Observable
                    .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                        t => castedView.Results.SelectionChanged += t,
                        t => castedView.Results.SelectionChanged -= t)
                    .Subscribe(SelectionChanged);

            base.OnViewReady(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (_selectionSubscription != null)
                    _selectionSubscription.Dispose();
            }

            base.OnDeactivate(close);
        }

        private void SelectionChanged(EventPattern<SelectionChangedEventArgs> e)
        {
            var listBox = e.Sender as ListBox;
            Contract.Assume(listBox != null);

            _globals.CheckinConnection = listBox.SelectedItem as Connection;
            listBox.SelectedItem = null;

            _navigationService
                .UriFor<CheckinViewModel>()
                .Navigate();
        }
    }
}
