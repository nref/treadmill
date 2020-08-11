using System;
using System.Globalization;
using Xamarin.Forms;

namespace Treadmill.Ui.Shared
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value as string);
        }
    }
}
