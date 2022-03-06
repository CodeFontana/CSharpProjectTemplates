namespace WpfUI.ViewModels;

public class HelloWorldViewModel : ViewModelBase
{
    private string _userInput;
    public string UserInput
    {
        get
        {
            return _userInput;
        }
        set
        {
            _userInput = value;
            OnPropertyChanged(nameof(UserInput));
        }
    }
}
