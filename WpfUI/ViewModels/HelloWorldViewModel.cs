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

            if (string.IsNullOrWhiteSpace(_userInput) == false)
            {
                Greeting = $"Hello, {_userInput}!";
            }
            else
            {
                Greeting = "";
            }

            OnPropertyChanged(nameof(UserInput));
            OnPropertyChanged(nameof(Greeting));
        }
    }

    public string Greeting { get; set; }
}
