using System;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITimeTable
    {
        IObservable<Location> GetLocations(string locationName);
        IObservable<Connection> GetConnections(string from, string to, DateTime departure);
    }
}