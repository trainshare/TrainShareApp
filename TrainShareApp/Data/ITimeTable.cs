using System;
using System.Threading.Tasks;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITimeTable
    {
        IObservable<Station> GetLocations(string locationName);
        Task<SearchResult> GetConnections(string from, string to, DateTime departure);
    }
}