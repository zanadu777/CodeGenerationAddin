using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Extensions
{
  public static class IVsHierarchyExtensions
  {
    public static Type GetExtObjectType(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint) VSConstants.VSITEMID.Root, (int) __VSHPROPID.VSHPROPID_ExtObject,
        out object extObject);

      return extObject?.GetType();
    }


    public static string GetProjectName(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint) VSConstants.VSITEMID.Root, (int) __VSHPROPID.VSHPROPID_Name, out object projectName);

      return projectName?.ToString();
    }


    public static List<string> GetCSharpFiles(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var project = hierarchy.ToProject();
      if (project == null)
      {
        return new List<string>();
      }

      var csharpFiles = new List<string>();
      foreach (EnvDTE.ProjectItem item in project.ProjectItems)
      {
        GetCSharpFiles(item, csharpFiles);
      }

      return csharpFiles;
    }

    public static EnvDTE.Project ToProject(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject);

      return extObject as EnvDTE.Project;
    }


    private static void GetCSharpFiles(EnvDTE.ProjectItem item, List<string> csharpFiles)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      for (short i = 0; i < item.FileCount; i++)
      {
        string filePath = item.FileNames[i];
        if (filePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
          csharpFiles.Add(filePath);
        }
      }

      foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
      {
        GetCSharpFiles(childItem, csharpFiles);
      }
    }
  }
}