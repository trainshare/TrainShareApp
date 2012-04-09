using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class GlobalStorage : JsonStorageHandler<Globals>
    {
        public override void Configure()
        {
            BuildInstruction("Globals").InAppSettings();
        }
    }
}