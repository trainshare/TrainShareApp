using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface IFacebookClient
    {
        /// <summary>
        /// LoginAsync and authorize the app
        /// </summary>
        /// <param name="viewBrowser">The browser where the user logs in</param>
        /// <returns>The new Token</returns>
        Task<Token> LoginAsync(WebBrowser viewBrowser);

        /// <summary>
        /// Dismiss the current Token
        /// </summary>
        /// <returns>A task that signals completion</returns>
        Task LogoutAsync();

        /// <summary>
        /// Get if the user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Get the current Token
        /// </summary>
        Token Token { get; }
    }
}