using WpfUI.Stores;

namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
}
