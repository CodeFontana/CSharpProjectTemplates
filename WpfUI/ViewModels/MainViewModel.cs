using System;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Stores;

namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        WelcomePageCommand = new NavigateCommand(_navigationStore, MakeHelloWorldViewModel);
        CounterPageCommand = new NavigateCommand(_navigationStore, MakeCounterViewModel);
    }

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

    public ICommand WelcomePageCommand { get; }
    public ICommand CounterPageCommand { get; }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private ViewModelBase MakeHelloWorldViewModel()
    {
        return new HelloWorldViewModel();
    }

    private ViewModelBase MakeCounterViewModel()
    {
        return new CounterViewModel();
    }
}
