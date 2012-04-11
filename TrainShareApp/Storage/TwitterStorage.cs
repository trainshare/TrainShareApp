using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class TwitterStorage : JsonStorageHandler<TwitterToken>
    {
        public override void Configure()
        {
            BuildInstruction("Twitter").InAppSettings();
        }
    }
}