using System.Globalization;

namespace Treadmill.Maui.Shared;

public class IntToStringConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString();
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => System.Convert.ToInt32(value as string);
}
