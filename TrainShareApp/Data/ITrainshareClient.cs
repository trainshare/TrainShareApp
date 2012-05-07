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
        /// <param name="checkin">The check in</param>
        /// <returns>A task that fires after the checkin</returns>
        Task Checkin(Checkin checkin);

        /// <summary>
        /// Checkout from the current connection
        /// </summary>
        /// <returns>A task that fires after the checkin</returns>
        Task Checkout();

        /// <summary>
        /// Send an additional access token and secret to the server so it can include friend graph
        /// </summary>
        /// <param name="token">The acces token</param>
        /// <returns>The created/updated users train share token</returns>
        Task<Token> LoginAsync(Token token);

        /// <summary>
        /// Get if the user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Get all friends on the same train as the user
        /// </summary>
        /// <returns>A task promise of a list of transharefirends</returns>
        Task<List<TrainshareFriend>> GetFriends();

        /// <summary>
        /// Get all previouse checkins
        /// </summary>
        /// <param name="count">The maximum count of previouse checkins</param>
        /// <returns>A task promise of a list of all previouse checkins</returns>
        Task<List<Checkin>> GetHistory(int count);

        /// <summary>
        /// Get the trainshare token
        /// </summary>
        Token Token { get; }

        /// <summary>
        /// Get the currently checked in connection
        /// </summary>
        Checkin CurrentCheckin { get; }
    }
}