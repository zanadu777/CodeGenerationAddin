using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SvgToXaml.IconLibrary
{
   public class SvgLibrary
   {
     private static Dictionary<string, DynamicSvg> Svgs = new Dictionary<string, DynamicSvg>();

     private Dictionary<SvgCacheKey, DrawingBrush> Brushes { get; set; }

     private Dictionary<SvgCacheKey, DrawingImage> DrawingImages { get; set; } =  new ();
     


    public DrawingImage DrawingImage(string svgName, params Color[] colors)
    {
      var key = new SvgCacheKey(svgName, colors);
      if (DrawingImages.ContainsKey(key))
        return DrawingImages[key];


      if (!Svgs.ContainsKey(svgName))
      {
        var svgText = ReadEmbeddedResource(svgName);
        var svg = new DynamicSvg(svgText);
        Svgs[svgName] = svg;
      }

      var cacheKey = new SvgCacheKey(svgName, colors);
      DrawingImages[cacheKey]= Svgs[svgName].Image(colors);

      return DrawingImages[cacheKey];

    }


    public DrawingBrush DrawingBrush(string svgName, params Color[] colors)
    {
      var image = DrawingImage(svgName, colors);

      var drawingBrush = new DrawingBrush(image.Drawing);
      if (drawingBrush.CanFreeze)
        drawingBrush.Freeze();


      return drawingBrush;

    }


    private string ReadEmbeddedResource(string resourceName)
    {
      resourceName += ".svg";
      var assembly = Assembly.GetExecutingAssembly();
      var resourcePath = assembly.GetManifestResourceNames()
        .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

      if (resourcePath == null)
        throw new FileNotFoundException("Resource not found", resourceName);

      using var stream = assembly.GetManifestResourceStream(resourcePath);
      using var reader = new StreamReader(stream);
      return reader.ReadToEnd();
    }
  }
}
