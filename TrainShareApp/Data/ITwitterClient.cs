using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITwitterClient
    {
        Task<TwitterToken> Login(WebBrowser viewBrowser);
        Task Logout();

        bool IsLoggedIn { get; }
    }
}