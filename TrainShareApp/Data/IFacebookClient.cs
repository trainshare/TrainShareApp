using System;
using System.Reactive;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface IFacebookClient
    {
        IObservable<OAuthToken> Login(WebBrowser viewBrowser);
        IObservable<Unit> Logout();
    }
}