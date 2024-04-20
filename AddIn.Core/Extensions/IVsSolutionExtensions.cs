using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AddIn.Core.Extensions
{
  public static  class IVsSolutionExtensions
  {

    public static List<IVsHierarchy> GetProjects(this IVsSolution solution)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var projects = new List<IVsHierarchy>();
      Guid emptyGuid = Guid.Empty;
      solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref emptyGuid, out IEnumHierarchies enumHierarchies);

      IVsHierarchy[] hierarchy = new IVsHierarchy[1];
      while (enumHierarchies.Next(1, hierarchy, out uint fetched) == VSConstants.S_OK && fetched == 1)
      {
        hierarchy[0].GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject);
        if (extObject is EnvDTE.Project project && project.Name != "Miscellaneous Files")
        {
          projects.Add(hierarchy[0]);
        }
      }

      return projects;
    }

    public static List<ProjectItem> GetProjectItems(this IVsSolution solution, Predicate<ProjectItem> filter)
    {
      var projectItems = new List<ProjectItem>();
      foreach (var project  in solution.GetProjects())
        projectItems.AddRange(project.ProjectItems(filter));

      return projectItems;
    }
  }
}
