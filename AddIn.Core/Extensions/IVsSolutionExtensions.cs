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

    public static List<ProjectItem> GetAllProjectItems(this IVsSolution solution)
    {
      return solution.GetProjectItems(x => true);
    }

    public static List<ProjectItem> GetProjectItems(this IVsSolution solution, Predicate<ProjectItem> filter)
    {
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
