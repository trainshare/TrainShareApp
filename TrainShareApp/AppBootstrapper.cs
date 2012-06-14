using TrainShareApp.Data;
using TrainShareApp.Logger;
using TrainShareApp.Model;
using TrainShareApp.ViewModels;

namespace TrainShareApp {
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Caliburn.Micro;

    public class AppBootstrapper : PhoneBootstrapper
    {
        PhoneContainer container;

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }

        protected override void Configure()
        {
            container = new PhoneContainer(RootFrame);

			container.RegisterPhoneServices();

            // Loggers
            container.Singleton<ILog, DebugLogger>();

            // Database
            container.Handler<DbDataContext>(
                ioc =>
                new DbDataContext(DbDataContext.DbConnectionString)
                {
                    DeferredLoadingEnabled = false,
                    ObjectTrackingEnabled = true
                });

            using (var context = (DbDataContext)container.GetInstance(typeof(DbDataContext), null))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                    context.SubmitChanges();
                }
            }

            // Clients
            container
                .Singleton<ITwitterClient, TwitterClient>()
                .Singleton<IFacebookClient, FacebookClient>()
                .Singleton<ITrainshareClient, TrainshareClient>()
                .Singleton<ITimeTable, TimeTable>();

            // ViewModels
            container
                .Singleton<MainPageViewModel>()
                .Singleton<MainViewModel>()
                .PerRequest<LoginViewModel>()
                .PerRequest<CheckinViewModel>()
                .Singleton<AccountsViewModel>()
                .Singleton<SearchViewModel>()
                .Singleton<SearchResultViewModel>();

            // This is important to force the container to build up all viewmodel at the beginning
            ////Console.WriteLine(container.GetInstance(typeof (MainViewModel), null));
            ////Console.WriteLine(container.GetInstance(typeof (LoginViewModel), null));
            ////Console.WriteLine(container.GetInstance(typeof (CheckinViewModel), null));
            ////Console.WriteLine(container.GetInstance(typeof (AccountsViewModel), null));
            ////Console.WriteLine(container.GetInstance(typeof (SearchResultViewModel), null));

            AddCustomConventions();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) => {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) => {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };
        }
    }
}
