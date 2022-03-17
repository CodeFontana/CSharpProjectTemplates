using System;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public MainViewModel(NavigationStore navigationStore,
                         NavigationService<HelloWorldViewModel> helloNav,
                         NavigationService<CounterViewModel> counterNav)
    {
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        WelcomePageCommand = new NavigateCommand<HelloWorldViewModel>(helloNav);
        CounterPageCommand = new NavigateCommand<CounterViewModel>(counterNav);
    }

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

    public ICommand WelcomePageCommand { get; }
    public ICommand CounterPageCommand { get; }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
