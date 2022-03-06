using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI.Controls;

public class AppDrawer : Control
{
    static AppDrawer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AppDrawer), new FrameworkPropertyMetadata(typeof(AppDrawer)));
    }

    public AppDrawer()
    {
        Width = 0;
    }

    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register("IsOpen", typeof(bool), typeof(AppDrawer),
            new PropertyMetadata(true, OnIsOpenPropertyChanged));

    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(AppDrawer),
            new PropertyMetadata(null));

    public FrameworkElement Content
    {
        get { return (FrameworkElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty OpenCloseDurationPropertyProperty =
        DependencyProperty.Register("OpenCloseDurationProperty", typeof(Duration), typeof(AppDrawer),
            new PropertyMetadata(defaultValue: Duration.Automatic));

    public Duration OpenCloseDurationProperty
    {
        get { return (Duration)GetValue(OpenCloseDurationPropertyProperty); }
        set { SetValue(OpenCloseDurationPropertyProperty, value); }
    }

    public static readonly DependencyProperty FallbackOpenWidthProperty =
        DependencyProperty.Register("FallbackOpenWidth", typeof(double), typeof(AppDrawer), 
            new PropertyMetadata(100.0));

    public double FallbackOpenWidth
    {
        get { return (double)GetValue(FallbackOpenWidthProperty); }
        set { SetValue(FallbackOpenWidthProperty, value); }
    }

    private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AppDrawer appDrawer)
        {
            appDrawer.OnIsOpenPropertyChanged();
        }
    }

    private void OnIsOpenPropertyChanged()
    {
        if (IsOpen)
        {
            OpenMenuAnimated();
        }
        else
        {
            CloseMenuAnimated();
        }
    }

    private void OpenMenuAnimated()
    {
        
        
        Content.Measure(new Size(MaxWidth, MaxHeight));
        double contentWidth = Content.DesiredSize.Width;
        DoubleAnimation openingAnimation = new(contentWidth, OpenCloseDurationProperty);
        BeginAnimation(WidthProperty, openingAnimation);
    }

    private void CloseMenuAnimated()
    {
        DoubleAnimation closingAnimation = new(0, OpenCloseDurationProperty);
        BeginAnimation(WidthProperty, closingAnimation);
    }

    private double GetDesiredContentWidth()
    {
        if (Content == null)
        {
            return FallbackOpenWidth;
        }

        Content.Measure(new Size(MaxWidth, MaxHeight));
        return Content.DesiredSize.Width;
    }
}
