using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniTrello.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        public ViewModelBase() { }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(isNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        string busyMessage;

        public bool isNotBusy => !IsBusy;
    }
}
