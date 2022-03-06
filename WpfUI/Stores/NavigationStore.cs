using WpfUI.ViewModels;

namespace WpfUI.Stores;

public class NavigationStore
{
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel 
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
        }
    }
}
