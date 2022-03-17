using System;
using WpfUI.Services;
using WpfUI.ViewModels;

namespace WpfUI.Commands;

public class NavigateCommand<T> : CommandBase where T : ViewModelBase
{
    private readonly NavigationService<T> _navigationService;

    public NavigateCommand(NavigationService<T> navigationService)
    {
        _navigationService = navigationService;
    }
    
    public override void Execute(object parameter)
    {
        _navigationService.Navigate();
    }
}
