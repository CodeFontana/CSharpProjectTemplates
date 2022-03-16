using System;
using WpfUI.Services;

namespace WpfUI.Commands;

public class NavigateCommand : CommandBase
{
    private readonly NavigationService _navigationService;

    public NavigateCommand(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }
    
    public override void Execute(object parameter)
    {
        _navigationService.Navigate();
    }
}
