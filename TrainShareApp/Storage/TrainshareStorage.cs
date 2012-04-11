using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class TrainshareStorage : JsonStorageHandler<TrainshareToken>
    {
        public override void Configure()
        {
            BuildInstruction("Trainshare").InAppSettings();
        }
    }
}
