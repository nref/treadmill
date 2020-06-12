using System;
using System.Globalization;
using Xamarin.Forms;

namespace Precor956i.Shared
{
    public class PausedToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Paused" : "Pause";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot convert {targetType} to bool");
        }
    }
}
