using System;
using System.Threading.Tasks;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITrainshareClient
    {
        Task<int> SendAccessToken(string network, string token, string tokenSecret);
        IObservable<TrainshareFriend> GetFriends();
        Task Checkin(int trainshareId, Connection connection);
    }
}