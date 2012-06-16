using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITimeTable
    {
        IObservable<IEnumerable<Station>> GetLocations(string locationName);
        Task<SearchResult> GetConnections(string from, string to, DateTime time, bool isArrival = false);
    }
}