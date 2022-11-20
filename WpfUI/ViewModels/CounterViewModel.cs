using System.Windows.Input;
using WpfUI.Commands;

namespace WpfUI.ViewModels;

public class CounterViewModel : ViewModelBase
{
    public CounterViewModel()
    {
        _currentCount = 0;
        IncrementCountCommand = new IncrementCountCommand(this);
        ResetCountCommand = new ResetCountCommand(this);
    }

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

    public ICommand ResetCountCommand { get; }
}
