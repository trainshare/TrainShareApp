using Caliburn.Micro;
using TrainShareApp.ViewModels;

namespace TrainShareApp.Storage
{
    public class MainViewStorage : StorageHandler<MainViewModel>
    {
        public override void Configure()
        {
            Property(vm => vm.From).InPhoneState();
            Property(vm => vm.To).InPhoneState();
            Property(vm => vm.Via).InPhoneState();
            Property(vm => vm.Time).InPhoneState();
        }
    }
}