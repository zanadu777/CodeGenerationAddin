using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        if (extObject is EnvDTE.Project project && project.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}" && project.Name != "Miscellaneous Files")
        {
          projects.Add(hierarchy[0]);
        }
      }

      return projects;
    }

    public static Project Project(this IVsSolution solution, string projectName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      Guid emptyGuid = Guid.Empty;
      solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref emptyGuid, out IEnumHierarchies enumHierarchies);

      IVsHierarchy[] hierarchy = new IVsHierarchy[1];
      while (enumHierarchies.Next(1, hierarchy, out uint fetched) == VSConstants.S_OK && fetched == 1)
      {
        hierarchy[0].GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject);
        if (extObject is EnvDTE.Project project && project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
        {
          return project;
        }
      }

      return null;
    }

    public static List<ProjectItem> GetAllProjectItems(this IVsSolution solution)
    {
      return solution.GetProjectItems(x => true);
    }

    public static List<ProjectItem> GetProjectItems(this IVsSolution solution, Predicate<ProjectItem> filter)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var projectItems = new List<ProjectItem>();
      foreach (var project  in solution.GetProjects())
        projectItems.AddRange(project.ProjectItems(filter));

      return projectItems;
    }


    public static Dictionary<string, ProjectItem> GetProjectItemsByFileName(this IVsSolution solution, List<string> fileNames)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var projectItemsByFileName = new Dictionary<string, ProjectItem>(StringComparer.OrdinalIgnoreCase);
      var fileNamesToFind = new HashSet<string>(fileNames, StringComparer.OrdinalIgnoreCase);

      foreach (var hierarchy in solution.GetProjects())
      {
        if (fileNamesToFind.Count == 0)
        {
          break; // All filenames have been found, so we can stop searching
        }

        hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject);

        if (extObject is EnvDTE.Project project)
        {
          foreach (ProjectItem projectItem in project.ProjectItems)
          {
            if (fileNamesToFind.Remove(projectItem.Name))
            {
              projectItemsByFileName[projectItem.Name] = projectItem;
            }
          }
        }
      }

      return projectItemsByFileName;
    }







  }
}
