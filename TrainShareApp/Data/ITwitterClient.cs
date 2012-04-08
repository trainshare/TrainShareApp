﻿using System;
using System.Reactive;
using Microsoft.Phone.Controls;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public interface ITwitterClient
    {
        IObservable<OAuthToken> Login(WebBrowser viewBrowser);
        IObservable<Unit> Logout();

        bool IsLoggedIn { get; }
    }
}