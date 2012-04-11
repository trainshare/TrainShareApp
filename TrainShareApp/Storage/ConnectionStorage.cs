using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class ConnectionStorage : JsonStorageHandler<Checkin>
    {
        public override void Configure()
        {
            BuildInstruction("Checkin").InAppSettings();
        }
    }
}
