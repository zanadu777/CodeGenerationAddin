using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Extensions
{
  public static  class PathHelper
  {
    public static bool IsValidDirectoryPath(string path)
    {
      try
      {
        string fullPath = Path.GetFullPath(path);

        // Check if the path has invalid characters
        if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
          return false;

        // Check if the path is rooted
        if (!Path.IsPathRooted(path))
          return false;

        return true;
      }
      catch (Exception)
      {
        // If the path is not valid, an exception will be thrown
        return false;
      }
    }


    public static string RelativePath(string fromPath, string toPath)
    {
      if (fromPath == toPath)
        return string.Empty;

      if (!IsValidDirectoryPath(fromPath))
        fromPath = Path.GetDirectoryName(fromPath);

      if (!fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        fromPath += Path.DirectorySeparatorChar;

      Uri fromUri = new Uri(fromPath);
      Uri toUri = new Uri(toPath);

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }
  }
}
