using WpfUI.ViewModels;

namespace WpfUI.Commands;

public class NavigateCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public NavigateCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
    
    public override void Execute(object parameter)
    {
        if (parameter is string viewType)
        {
            switch (viewType)
            {
                case "Home":
                    _mainViewModel.CurrentViewModel = new HelloWorldViewModel();
                    break;
                case "Counter":
                    _mainViewModel.CurrentViewModel = new CounterViewModel();
                    break;
                default:
                    break;
            }
        }
    }
}
