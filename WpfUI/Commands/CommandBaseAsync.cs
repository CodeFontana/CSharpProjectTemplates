using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WpfUI.Commands;

public abstract class CommandBaseAsync : CommandBase
{
    private bool _isExecuting;

    private bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return !IsExecuting && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _ = RunAsync(parameter);
    }

    private async Task RunAsync(object? parameter)
    {
        IsExecuting = true;
        try
        {
            await ExecuteAsync(parameter);
        }
        catch (Exception ex)
        {
            Trace.TraceError("[{0}] Async command failed: {1}", GetType().Name, ex);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    public abstract Task ExecuteAsync(object? parameter);
}
