using System.Collections.Generic;
using System.Windows.Media;

namespace SvgToXaml.Extensions
{
    public static class DrawingGroupExtensions
    {
        public static List<Color> GetColors(this DrawingGroup drawingGroup)
        {
            var colors = new List<Color>();
            var stack = new Stack<Drawing>();

            stack.Push(drawingGroup);

            while (stack.Count > 0)
            {
                var currentDrawing = stack.Pop();

                if (currentDrawing is GeometryDrawing geometryDrawing)
                {
                    if (geometryDrawing.Brush is SolidColorBrush solidColorBrush)
                    {
                        colors.Add(solidColorBrush.Color);
                    }
                    if (geometryDrawing.Pen?.Brush is SolidColorBrush penBrush)
                    {
                        colors.Add(penBrush.Color);
                    }
                }
                else if (currentDrawing is DrawingGroup currentGroup)
                {
                    foreach (var child in currentGroup.Children)
                    {
                        stack.Push(child);
                    }
                }
            }

            return colors;
        }
    }
}
