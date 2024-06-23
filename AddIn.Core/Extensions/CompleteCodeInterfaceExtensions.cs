using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.Extensions
{
  public static  class CompleteCodeInterfaceExtensions
  {

    public static  CodeInterface WithFileName(this IEnumerable<CompleteCodeInterface> completeCodeInterfaces, string fileName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      return completeCodeInterfaces
        .SelectMany(c => c.CodeInterfaces)
        .FirstOrDefault(c => System.IO.Path.GetFileNameWithoutExtension(c.ProjectItem.FileNames[0]) == System.IO.Path.GetFileNameWithoutExtension(fileName));
    }

    public static List<CodeInterface> WithFileNames(this IEnumerable<CompleteCodeInterface> completeCodeInterfaces, IEnumerable<string> fileNames)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      return completeCodeInterfaces
        .SelectMany(c => c.CodeInterfaces)
        .Where(c => fileNames.Contains(System.IO.Path.GetFileNameWithoutExtension(c.ProjectItem.FileNames[0])))
        .ToList();
    }
  }
}
