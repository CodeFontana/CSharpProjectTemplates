using System.ComponentModel;
using WpfUI.ViewModels;

namespace WpfUI.Commands;

public class ResetCountCommand : CommandBase
{
    private readonly CounterViewModel _counterViewModel;

    public ResetCountCommand(CounterViewModel counterViewModel)
    {
        _counterViewModel = counterViewModel;
        _counterViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _counterViewModel.CurrentCount > 0;
    }

    public override void Execute(object? parameter)
    {
        _counterViewModel.CurrentCount = 0;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CounterViewModel.CurrentCount))
        {
            OnCanExecutedChanged();
        }
    }
}
