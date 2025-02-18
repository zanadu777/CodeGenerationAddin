using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SvgToXaml.Extensions
{
  public static class DrawingImageExtensions
  {
    public static DrawingBrush DrawingBrush(this DrawingImage image)
    {

      var drawingBrush = new DrawingBrush(image.Drawing);
      if (drawingBrush.CanFreeze)
        drawingBrush.Freeze();
 
      return drawingBrush;
    }

    public static void  ToPng(DrawingImage drawingImage, string filePath, int width, int height)
    {
      if (drawingImage == null)
        throw new ArgumentNullException(nameof(drawingImage));

      if (string.IsNullOrEmpty(filePath))
        throw new ArgumentNullException(nameof(filePath));

      // Create a RenderTargetBitmap to render the DrawingImage
      var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

      // Create a DrawingVisual to render the DrawingImage
      var drawingVisual = new DrawingVisual();
      using (var drawingContext = drawingVisual.RenderOpen())
      {
        drawingContext.DrawImage(drawingImage, new Rect(0, 0, width, height));
      }

      // Render the DrawingVisual to the RenderTargetBitmap
      renderTarget.Render(drawingVisual);

      // Create a PngBitmapEncoder to save the RenderTargetBitmap to a PNG file
      var pngEncoder = new PngBitmapEncoder();
      pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

      // Save the PNG file
      using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        pngEncoder.Save(fileStream);
      }
    }


    public static void ToIco(this DrawingImage drawingImage, string filePath )
    {
      var sizes = new[] {16, 24, 32, 48, 64, 128, 256};
      ToIco(drawingImage, filePath, sizes);
    }
    public static void ToIco(this DrawingImage drawingImage, string filePath, int[] sizes)
    {
      if (drawingImage == null)
        throw new ArgumentNullException(nameof(drawingImage));

      if (string.IsNullOrEmpty(filePath))
        throw new ArgumentNullException(nameof(filePath));

      using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        using (var binaryWriter = new BinaryWriter(fileStream))
        {
          // Write ICO header
          binaryWriter.Write((short)0); // Reserved
          binaryWriter.Write((short)1); // ICO type
          binaryWriter.Write((short)sizes.Length); // Number of images

          var imageData = new List<byte[]>();
          var imageOffsets = new List<int>();

          foreach (var size in sizes)
          {
            var bitmapSource = RenderDrawingImageToBitmapSource(drawingImage, size, size);
            var pngData = EncodeBitmapSourceToPng(bitmapSource);

            imageData.Add(pngData);
            imageOffsets.Add((int)fileStream.Position + 16 * sizes.Length);

            // Write image directory entry
            binaryWriter.Write((byte)size); // Width
            binaryWriter.Write((byte)size); // Height
            binaryWriter.Write((byte)0); // Color palette
            binaryWriter.Write((byte)0); // Reserved
            binaryWriter.Write((short)1); // Color planes
            binaryWriter.Write((short)32); // Bits per pixel
            binaryWriter.Write(pngData.Length); // Image size
            binaryWriter.Write(0); // Image offset (placeholder)
          }

          for (int i = 0; i < sizes.Length; i++)
          {
            var currentPosition = fileStream.Position;
            fileStream.Position = 6 + 16 * i + 12;
            binaryWriter.Write(imageOffsets[i]);
            fileStream.Position = currentPosition;
            binaryWriter.Write(imageData[i]);
          }
        }
      }
    }

    private static BitmapSource RenderDrawingImageToBitmapSource(DrawingImage drawingImage, int width, int height)
    {
      var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
      var drawingVisual = new DrawingVisual();
      using (var drawingContext = drawingVisual.RenderOpen())
      {
        drawingContext.DrawImage(drawingImage, new Rect(0, 0, width, height));
      }
      renderTarget.Render(drawingVisual);
      return renderTarget;
    }

    private static byte[] EncodeBitmapSourceToPng(BitmapSource bitmapSource)
    {
      using (var memoryStream = new MemoryStream())
      {
        var pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        pngEncoder.Save(memoryStream);
        return memoryStream.ToArray();
      }
    }


  }
}
