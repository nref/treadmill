using System.Globalization;

namespace Treadmill.Maui.Shared;

public class BoolToColorConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? (Color)parameter : Colors.Transparent;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException($"Cannot convert {targetType} to bool");
}
