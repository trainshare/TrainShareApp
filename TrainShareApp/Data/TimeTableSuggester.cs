using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Phone.Reactive;
using TrainShareApp.Extension;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class TimeTableSuggester
    {
        private readonly ITimeTable _timeTable;

        public TimeTableSuggester(ITimeTable timeTable)
        {
            _timeTable = timeTable;

            InputString = string.Empty;
            ItemsSource = Enumerable.Empty<Station>();
        }

        public string InputString { get; private set; }

        public IEnumerable<Station> ItemsSource { get; private set; }

        public void Reset()
        {
            InputString = string.Empty;
            Update(InputString);
        }

        public void Input(int start, int selectionLength, string c)
        {
            InputString = InputString.Remove(start, selectionLength).Insert(start, c);
            Update(InputString);
        }

        public void Delete(int start, int selectionLength)
        {
            InputString = InputString.Remove(start, selectionLength);
            Update(InputString);
        }

        public bool HasSuggestions
        {
            get { return ItemsSource.Any(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime _latestRequest;

        public async void Update(string fragment)
        {
            if (string.IsNullOrWhiteSpace(fragment) || fragment.Length < 3)
            { // If the fragment is smaller than 3 characters just clean up all suggestions
                return;
            }

            var requestTime = DateTime.Now;

            if (_latestRequest.Add(TimeSpan.FromMilliseconds(100)) > requestTime)
            { // Dont request faster than every 100ms from the server
                return;
            }

            _latestRequest = requestTime;
            var response = await _timeTable.GetLocations(InputString).ToTask();

            if (requestTime < _latestRequest)
            { // Someone was faster
                return;
            }

            ItemsSource = Order(response, fragment).ToList();
        }

        private IEnumerable<Station> Order(IEnumerable<Station> stations, string fragment)
        {
            if (stations == null) return Enumerable.Empty<Station>();

            var array =
                stations
                    .Select(
                        station =>
                        new
                        {
                            Norm = station.Name.LevensteinNorm(fragment),
                            Station = station
                        })
                    .ToArray();

            Array.Sort(array, (a, b) => a.Norm - b.Norm < 0 ? -1 : 1);

            return array.Select(station => station.Station);
        }
    }
}