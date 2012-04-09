using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface IFacebookClient
    {
        Task<FacebookToken> Login(WebBrowser viewBrowser);
        Task LogoutAsync();
        bool IsLoggedIn { get; }
    }
}