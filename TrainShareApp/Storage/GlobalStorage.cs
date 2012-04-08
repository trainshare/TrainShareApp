using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class GlobalStorage : StorageHandler<Globals>
    {
        public override void Configure()
        {
            Property(globals => globals.TrainshareId).InAppSettings();

            Property(globals => globals.TwitterId).InAppSettings();
            Property(globals => globals.TwitterName).InAppSettings();
            Property(globals => globals.TwitterToken).InAppSettings();
            Property(globals => globals.TwitterSecret).InAppSettings();

            Property(globals => globals.FacebookId).InAppSettings();
            Property(globals => globals.FacebookName).InAppSettings();
            Property(globals => globals.FacebookToken).InAppSettings();
        }
    }
}