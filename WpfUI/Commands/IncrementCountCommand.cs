namespace WpfUI.Commands;

public class IncrementCountCommand : CommandBase
{
    private readonly CounterViewModel _counterViewModel;

    public IncrementCountCommand(CounterViewModel counterViewModel)
    {
        _counterViewModel = counterViewModel;
    }

    public override void Execute(object parameter)
    {
        _counterViewModel.CurrentCount += 1;
    }
}
