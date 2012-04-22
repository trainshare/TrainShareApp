using Caliburn.Micro;
using TrainShareApp.Data;

namespace TrainShareApp.Storage
{
    public class HistoryStorage : JsonStorageHandler<CheckinHistory>
    {
        public override void Configure()
        {
            BuildInstruction("History").InAppSettings();
        }
    }
}