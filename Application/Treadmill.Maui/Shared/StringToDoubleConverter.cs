using System.Globalization;

namespace Treadmill.Maui.Shared;

public class StringToDoubleConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => System.Convert.ToDouble(value as string);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString();
}
