using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Storage
{
    public class FacebookStorage : JsonStorageHandler<FacebookToken>
    {
        public override void Configure()
        {
            BuildInstruction("Facebook").InAppSettings();
        }
    }
}
