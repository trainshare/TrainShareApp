using System;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITrainshareRepository
    {
        IObservable<string> SendAccessToken(string network, string token, string tokenSecret);
        IObservable<TrainshareFriend> GetFriends();
    }
}