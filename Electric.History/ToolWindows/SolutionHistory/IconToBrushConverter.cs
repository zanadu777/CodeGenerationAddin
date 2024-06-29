using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AddIn.Core.Icons;

namespace Electric.History.ToolWindows.SolutionHistory
{
  public class IconToBrushConverter : IValueConverter
  {
    public IconToBrushConverter()
    {
      colorMap["gold"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFBA00"));
      colorMap["gold"].Freeze();
      colorMap["orange"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFB6442"));
      colorMap["orange"].Freeze();
      colorMap["aqua"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF26B9D9"));
      colorMap["aqua"].Freeze();
      colorMap["lavender"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCA96EB"));
      colorMap["lavender"].Freeze();
      colorMap["red"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD41C2C"));
      colorMap["red"].Freeze();
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is null)
        return Brushes.Transparent;

      if (value is Icon icon)
      {
        if (icon.Shape == "none")
        {
          return Brushes.Transparent;
        }
        else if (icon.Shape == "bolt")
        {
          var boltBrush = BrushBuilder.BuildBoltDrawingBrush(colorMap[icon.Color1]);
          return boltBrush;
        }
        else if (icon.Shape == "bolts")
        {
          var boltBrush = BrushBuilder.BuildBoltsDrawingBrush(colorMap[icon.Color1], colorMap[icon.Color2]);
          return boltBrush;
        }
        else if (icon.Shape == "circleBolt")
        {
          var boltBrush = BrushBuilder.BuildCircleBolt(colorMap[icon.Color1], colorMap[icon.Color2]);
          return boltBrush;
        }
        else if (icon.Shape == "squareBolt")
        {
          var boltBrush = BrushBuilder.BuildSquareBolt(colorMap[icon.Color1], colorMap[icon.Color2]);
          return boltBrush;
        }


        //var color1 = colorMap[icon.Color1] ;

        //var color2 = string.IsNullOrWhiteSpace(icon.Color2) ? colorMap[icon.Color1] : colorMap[icon.Color2];
        //return new LinearGradientBrush(color1, color2, 0);
      }
      return Brushes.Transparent; // Fallback in case of conversion failure
    }

    private Dictionary<string, Brush> colorMap = new();

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Conversion back is not supported/needed in this scenario
      throw new NotImplementedException();
    }
  }
}
