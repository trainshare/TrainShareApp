using System;
using System.Reflection;
using Microsoft.Phone.Tasks;

namespace TrainShareApp.ViewModels
{
    public class AboutViewModel
    {
        public string Version
        {
            get
            {
                try
                {
                    var versionRaw = Assembly.GetExecutingAssembly().FullName.Split('=')[1];
                    return versionRaw.Split(',')[0];
                }
                catch (Exception)
                {
                    return "Unknown";
                }
            }
        }

        public void Review()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }
    }
}