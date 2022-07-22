namespace WpfUI.Controls;

public class AppDrawerItem : RadioButton
{
    static AppDrawerItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AppDrawerItem), 
            new FrameworkPropertyMetadata(typeof(AppDrawerItem)));
    }
}
