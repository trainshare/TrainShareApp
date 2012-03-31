using Microsoft.Phone.Controls;

namespace TrainShareApp.Data
{
    public interface ITwitterClient
    {
        bool HasToken { get; }
        string AccessToken { get; }
        string AccesTokenSecret { get; }

        void Login(WebBrowser viewBrowser);
    }
}