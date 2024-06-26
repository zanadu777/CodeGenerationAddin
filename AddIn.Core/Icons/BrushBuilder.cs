using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AddIn.Core.Icons
{
  public class BrushBuilder
  {

    public static DrawingBrush BuildCircleBolt(string boltColor, string circleColor)
    {
      // Create the outer DrawingGroup
      var outerDrawingGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Create the first inner DrawingGroup for the circle
      var circleDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1.19909, 0, 0, 1.19465, -97.7867, -3.05097)
      };
      var circleGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(circleColor)),
        Geometry = new EllipseGeometry(new Point(87.418, 8.351), 4.134, 4.134)
      };
      circleDrawingGroup.Children.Add(circleGeometryDrawing);

      // Create the second inner DrawingGroup for the bolt
      var boltDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1, 0, 0, 0.978939, 0.491352, -0.990068)
      };
      var boltGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      boltDrawingGroup.Children.Add(boltGeometryDrawing);

      // Add the inner DrawingGroups to the outer DrawingGroup
      outerDrawingGroup.Children.Add(circleDrawingGroup);
      outerDrawingGroup.Children.Add(boltDrawingGroup);

      // Create the DrawingBrush using the outer DrawingGroup
      var drawingBrush = new DrawingBrush(outerDrawingGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }

    public static DrawingBrush BuildSquareBolt(string boltColor, string squareColor)
    {
      // Create the outer DrawingGroup
      var outerDrawingGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Create the first inner DrawingGroup for the square
      var squareDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1.23355, 0, 0, 1.29813, -61.8575, -3.55647)
      };
      var squareGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(squareColor)),
        Geometry = new RectangleGeometry(new Rect(51.754, 4.176, 8.192, 7.864))
      };
      squareDrawingGroup.Children.Add(squareGeometryDrawing);

      // Create the second inner DrawingGroup for the bolt
      var boltDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1, 0, 0, 0.978939, 0.527086, -0.990068)
      };
      var boltGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      boltDrawingGroup.Children.Add(boltGeometryDrawing);

      // Add the inner DrawingGroups to the outer DrawingGroup
      outerDrawingGroup.Children.Add(squareDrawingGroup);
      outerDrawingGroup.Children.Add(boltDrawingGroup);

      // Create the DrawingBrush using the outer DrawingGroup
      var drawingBrush = new DrawingBrush(outerDrawingGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }


    public static DrawingBrush BuildBoltsDrawingBrush(string firstBoltColor, string secondBoltColor)
    {
      // Create the outer DrawingGroup with the specified clip geometry
      var outerDrawingGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Create the first bolt DrawingGroup
      var firstBoltDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1, 0, 0, 0.996296, -2.97291, -1.10532)
      };
      var firstBoltGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(firstBoltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      firstBoltDrawingGroup.Children.Add(firstBoltGeometryDrawing);

      // Create the second bolt DrawingGroup
      var secondBoltDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1, 0, 0, 0.996296, 4.02709, -1.10532)
      };
      var secondBoltGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(secondBoltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      secondBoltDrawingGroup.Children.Add(secondBoltGeometryDrawing);

      // Add the bolt DrawingGroups to the outer DrawingGroup
      outerDrawingGroup.Children.Add(firstBoltDrawingGroup);
      outerDrawingGroup.Children.Add(secondBoltDrawingGroup);

      // Create the DrawingBrush using the outer DrawingGroup
      var drawingBrush = new DrawingBrush(outerDrawingGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }



    public static DrawingBrush BuildBoltDotsDrawingBrush(string boltColor, string dotsColor)
    {
      // Create the outer DrawingGroup with the specified clip geometry
      var outerDrawingGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Create the bolt DrawingGroup
      var boltDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1, 0, 0, 0.996296, -2.88173, -1.12108)
      };
      var boltGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      boltDrawingGroup.Children.Add(boltGeometryDrawing);

      // Create the first dot DrawingGroup
      var firstDotDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1.05988, 0, 0, 1.04291, -34.2377, -6.61091)
      };
      var firstDotGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(dotsColor)),
        Geometry = new EllipseGeometry(new Point(40.426, 13.449), 1.426, 1.46)
      };
      firstDotDrawingGroup.Children.Add(firstDotGeometryDrawing);

      // Create the second dot DrawingGroup
      var secondDotDrawingGroup = new DrawingGroup
      {
        Opacity = 1,
        Transform = new MatrixTransform(1.05988, 0, 0, 1.04291, -30.376, -6.61091)
      };
      var secondDotGeometryDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(dotsColor)),
        Geometry = new EllipseGeometry(new Point(40.426, 13.449), 1.426, 1.46)
      };
      secondDotDrawingGroup.Children.Add(secondDotGeometryDrawing);

      // Add the bolt and dots DrawingGroups to the outer DrawingGroup
      outerDrawingGroup.Children.Add(boltDrawingGroup);
      outerDrawingGroup.Children.Add(firstDotDrawingGroup);
      outerDrawingGroup.Children.Add(secondDotDrawingGroup);

      // Create the DrawingBrush using the outer DrawingGroup
      var drawingBrush = new DrawingBrush(outerDrawingGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }

    public static DrawingBrush BuildBoltHamburgerDrawingBrush(string boltColor, string hamburgerColor)
    {
      // Create the main drawing group
      var mainGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z"),
        Transform = new TranslateTransform(-3.55271E-15, -0.984241)
      };

      // Bolt drawing
      var boltDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      boltDrawing.Geometry.Transform = new ScaleTransform(1, 0.996296, -2.90888, -0.121076);
      mainGroup.Children.Add(boltDrawing);

      // Hamburger lines drawing
      var lineTransforms = new[]
      {
      new TranslateTransform(-0.780485, -0.444151),
      new TranslateTransform(-0.888957, 3.79872),
      new TranslateTransform(-0.690092, -4.02235)
    };
      foreach (var transform in lineTransforms)
      {
        var lineDrawing = new GeometryDrawing
        {
          Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(hamburgerColor)),
          Geometry = new RectangleGeometry(new Rect(8.898, 7.045, 6.091, 1.982))
        };
        lineDrawing.Geometry.Transform = transform;
        mainGroup.Children.Add(lineDrawing);
      }

      // Create and return the DrawingBrush
      var drawingBrush = new DrawingBrush(mainGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }
    public static DrawingBrush BuildBoltDotDrawingBrush(string boltColor, string dotsColor)
    {
      // Create the main drawing group
      var mainGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Bolt drawing
      var boltDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      boltDrawing.Geometry.Transform = new ScaleTransform(1, 0.996296, -2.90888, -0.121076);
      mainGroup.Children.Add(boltDrawing);

      // Dots drawing
      // Assuming a simple representation of dots as ellipses
      var dotPositions = new[] { new Point(4, 4), new Point(6, 6), new Point(8, 8) }; // Example positions
      foreach (var dotPosition in dotPositions)
      {
        var dotDrawing = new GeometryDrawing
        {
          Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(dotsColor)),
          Geometry = new EllipseGeometry(dotPosition, 1, 1) // Example size
        };
        mainGroup.Children.Add(dotDrawing);
      }

      // Create and return the DrawingBrush
      var drawingBrush = new DrawingBrush(mainGroup)
      {
        Stretch = Stretch.Uniform
      };

      return drawingBrush;
    }

    public static DrawingImage BuildBoltDrawingImage(string boltColor)
    {
      // Create the main drawing group
      var mainGroup = new DrawingGroup
      {
        ClipGeometry = Geometry.Parse("M0,0 V14 H15 V0 H0 Z")
      };

      // Bolt drawing
      var boltDrawing = new GeometryDrawing
      {
        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(boltColor)),
        Geometry = Geometry.Parse("F0 M15,14z M0,0z M8.017,1.109L2.973,9.101 6.059,9.118 4.966,15.151 10.046,7.09 7.029,7.142 8.017,1.109z")
      };
      // Adjust the bolt's position and scale if necessary
      boltDrawing.Geometry.Transform = new ScaleTransform(1, 0.996296, -2.90888, -0.121076);
      mainGroup.Children.Add(boltDrawing);

      // Create and return the DrawingImage
      var drawingImage = new DrawingImage(mainGroup);

      return drawingImage;
    }

  }
}
