using System.Windows;
using System.Windows.Controls;

namespace WpfUI.Controls;

public class AppDrawerItem : RadioButton
{
    static AppDrawerItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AppDrawerItem), new FrameworkPropertyMetadata(typeof(AppDrawerItem)));
    }
}
