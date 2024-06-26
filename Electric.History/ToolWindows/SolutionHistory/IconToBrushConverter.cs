using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Electric.History.ToolWindows.SolutionHistory
{
  public class IconToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is null )
        return Brushes.Transparent;

      if (value is Icon icon)
      {
        var color1 = (Color)ColorConverter.ConvertFromString(icon.Color1);
        var color2 = (Color)ColorConverter.ConvertFromString(icon.Color2);
        return new LinearGradientBrush(color1, color2, 0);
      }
      return Brushes.Transparent; // Fallback in case of conversion failure
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Conversion back is not supported/needed in this scenario
      throw new NotImplementedException();
    }
  }
}
