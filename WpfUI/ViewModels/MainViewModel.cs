using System;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        WelcomePageCommand = new NavigateCommand(new NavigationService(_navigationStore, () => { return new HelloWorldViewModel(); }));
        CounterPageCommand = new NavigateCommand(new NavigationService(_navigationStore, () => { return new CounterViewModel(); }));
    }

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

    public ICommand WelcomePageCommand { get; }
    public ICommand CounterPageCommand { get; }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
