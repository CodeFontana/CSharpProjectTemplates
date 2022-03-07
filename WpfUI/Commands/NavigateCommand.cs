using System;
using WpfUI.Stores;
using WpfUI.ViewModels;

namespace WpfUI.Commands;

public class NavigateCommand : CommandBase
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<ViewModelBase> _createViewModel;

    public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
    {
        _navigationStore = navigationStore;
        _createViewModel = createViewModel;
    }
    
    public override void Execute(object parameter)
    {
        _navigationStore.CurrentViewModel = _createViewModel();
    }
}
