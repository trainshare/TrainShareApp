using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface IFacebookClient
    {
        Task<OAuthToken> Login(WebBrowser viewBrowser);
        Task Logout();
    }
}