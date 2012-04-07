using System;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITwitterClient
    {
        IObservable<TwitterToken> Login(WebBrowser viewBrowser);
    }
}