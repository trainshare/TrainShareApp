using System;
using System.Threading.Tasks;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITrainshareClient
    {
        Task<string> SendAccessToken(string network, string token, string tokenSecret);
        IObservable<TrainshareFriend> GetFriends(string trainshareId);
        IObservable<TrainshareFriend> Checkin(string trainshareId, Connection connection);
    }
}