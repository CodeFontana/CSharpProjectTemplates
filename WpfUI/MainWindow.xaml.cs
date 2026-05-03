using System.Windows;
using WpfUI.ViewModels;

namespace WpfUI;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}
