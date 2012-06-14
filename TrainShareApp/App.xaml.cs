using System;
using TrainShareApp.Model;

namespace TrainShareApp
{
    using System.Windows;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        public Connection SearchSelection { get; set; }

        /// <summary>
        /// This specifies how long into the past the user can search until we add an additional day => until we search tomorrow.
        /// </summary>
        public static readonly TimeSpan SearchTimeTolerance = TimeSpan.FromMinutes(10);

        public static App Instance { get { return Application.Current as App; } }
    }
}
