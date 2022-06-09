using System;
using System.Globalization;

namespace Treadmill.Maui.Shared;

public class BoolInverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
}
