using System.Collections.Generic;
using System.Threading.Tasks;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITrainshareClient
    {
        /// <summary>
        /// Checkin for a connection
        /// </summary>
        /// <param name="connection">The connection to check in</param>
        /// <param name="position">The position in the train (between 0 and 10) </param>
        /// <returns>A task that fires after the checkin</returns>
        Task Checkin(Connection connection, int position);

        /// <summary>
        /// Send an additional access token and secret to the server so it can include friend graph
        /// </summary>
        /// <param name="network">The network, atm either 'facebook' or 'twitter'</param>
        /// <param name="token">The acces token</param>
        /// <param name="tokenSecret">The access token secret</param>
        /// <returns>The created/updated users train share token</returns>
        Task<TrainshareToken> SendAccessToken(string network, string token, string tokenSecret);

        /// <summary>
        /// Get all friends on the same train as the user
        /// </summary>
        /// <returns>A task promise of a list of transharefirends</returns>
        Task<IEnumerable<TrainshareFriend>> GetFriends();

        /// <summary>
        /// Get the trainshare token
        /// </summary>
        TrainshareToken Token { get; }

        /// <summary>
        /// Get the currently checked in connection
        /// </summary>
        Checkin CurrentCheckin { get; }
    }
}