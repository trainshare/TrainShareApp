using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITwitterClient
    {
        /// <summary>
        /// Login and authorize the app
        /// </summary>
        /// <param name="viewBrowser">The browser where the user logs in</param>
        /// <returns>The new TwitterToken</returns>
        Task<TwitterToken> Login(WebBrowser viewBrowser);

        /// <summary>
        /// Dismiss the current TwitterToken
        /// </summary>
        /// <returns>A task that signals completion</returns>
        Task LogoutAsync();

        /// <summary>
        /// Get if the user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Get the current TwitterToken
        /// </summary>
        TwitterToken Token { get; }
    }
}