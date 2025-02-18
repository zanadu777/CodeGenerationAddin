using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SvgToXaml.Extensions;

namespace SvgToXaml.IconLibrary
{
  public class DynamicSvg
  {

    public string SvgText { get; set; }




    public DynamicSvg(string svgText)
    {
      SvgText = svgText;
      drawing = LoadSvgDrawingFromText(SvgText);
    }

    private readonly DrawingGroup drawing;

    public DrawingImage Image(params Color[] colors)
    {


      var existingColors = drawing.GetColors();

      for (int i = 0; i < existingColors.Count && i < colors.Length; i++)
      {
        ReplaceColor(drawing, existingColors[i], colors[i]);
      }

      return new DrawingImage(drawing);
    }

    private DrawingGroup? LoadSvgDrawingFromText(string svgContent)
    {
      var settings = new WpfDrawingSettings();
      var reader = new FileSvgReader(settings);
      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent)))
      {
        return reader.Read(stream);
      }
    }



    private void ReplaceColor(DrawingGroup drawingGroup, Color oldColor, Color newColor)
    {
      foreach (var drawing in drawingGroup.Children)
      {
        if (drawing is GeometryDrawing geometryDrawing)
        {
          if (geometryDrawing.Brush is SolidColorBrush solidColorBrush && solidColorBrush.Color == oldColor)
          {
            geometryDrawing.Brush = new SolidColorBrush(newColor);
          }
          if (geometryDrawing.Pen?.Brush is SolidColorBrush penBrush && penBrush.Color == oldColor)
          {
            geometryDrawing.Pen.Brush = new SolidColorBrush(newColor);
          }
        }
        else if (drawing is DrawingGroup childGroup)
        {
          ReplaceColor(childGroup, oldColor, newColor);
        }
      }
    }
  }
}
