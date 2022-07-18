﻿using System;
using System.Globalization;

namespace WpfUI.Converters;

public class InverseBooleanConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(bool))
        {
            throw new InvalidOperationException("The target must be a boolean type");
        }

        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
