﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Precor956i.Shared
{
    public class PausedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Color.Orange : Color.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot convert {targetType} to bool");
        }
    }
}