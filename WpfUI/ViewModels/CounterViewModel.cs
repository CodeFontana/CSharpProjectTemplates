using System.Windows.Input;

namespace WpfUI.ViewModels;

public class CounterViewModel : ViewModelBase
{
    private int _currentCount;
    public int CurrentCount
    {
        get
        {
            return _currentCount;
        }
        set
        {
            _currentCount = value;
            OnPropertyChanged(nameof(CurrentCount));
        }
    }

    public ICommand IncrementCountCommand { get; }
}
