using System;
using System.Diagnostics;
using Caliburn.Micro;
using TrainShareApp.Data;
using TrainShareApp.Views;

namespace TrainShareApp.ViewModels
{
    public class AccountsViewModel : Screen
    {
        private readonly ILog _logger;
        private readonly IFacebookClient _facebookClient;
        private readonly ITwitterClient _twitterClient;

        public AccountsViewModel()
        {
            Debug.Assert(Execute.InDesignMode, "Default constructor can only be called to generate design data.");
        }

        public AccountsViewModel(
            ILog logger,
            IFacebookClient facebookClient,
            ITwitterClient twitterClient)
        {
            _logger = logger;
            _facebookClient = facebookClient;
            _twitterClient = twitterClient;
        }

        public bool Loading { get; set; }

        public async void Facebook()
        {
            View.Browser.NavigateToString(string.Empty);
            View.Window.IsOpen = true;

            try
            {
                await _facebookClient.LoginAsync(View.Browser);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                View.Window.IsOpen = false;
            }
        }

        public async void Twitter()
        {
            View.Browser.NavigateToString(string.Empty);
            View.Window.IsOpen = true;

            try
            {
                await _twitterClient.LoginAsync(View.Browser);
            }
            catch(Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                View.Window.IsOpen = false;
            }
        }

        private AccountsView View
        {
            get { return GetView() as AccountsView; }
        }
    }
}
