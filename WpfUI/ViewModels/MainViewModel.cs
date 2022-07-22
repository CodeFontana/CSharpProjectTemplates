namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        NavigateCommand = new NavigateCommand(this);
        NavigateCommand.Execute("Home");
    }

    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get
        {
            return _currentViewModel;
        }

        set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public ICommand NavigateCommand { get; set; }
}
