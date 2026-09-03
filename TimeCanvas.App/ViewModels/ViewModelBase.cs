using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeCanvas.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;
}
