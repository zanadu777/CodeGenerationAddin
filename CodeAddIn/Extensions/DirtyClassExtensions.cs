using AddIn.Core.Extensions;
using CodeModel;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Extensions
{
  public static class DirtyClassExtensions
  {
    public static List<DirtyClass> DirtyClasses(this EnvDTE.Solution solution)
    {
      List<DirtyClass> dirtyClasses = new List<DirtyClass>();
      foreach (EnvDTE.Project project in solution.Projects(EProjectFilter.ExcludeVsProjectKindMisc))
        dirtyClasses.AddRange(project.DirtyClasses());

      return dirtyClasses;
    }

    public static List<DirtyClass> DirtyClasses(this Project project)
    {
      List<DirtyClass> dirtyClasses = new List<DirtyClass>();
      foreach (EnvDTE.ProjectItem item in project.ProjectItems)
      {
        if (item.IsDirty)
        {
          DirtyClass dirtyClass = new DirtyClass
          {
            Name = item.Name,
            FullName = item.get_FileNames(1),
            LastModified = System.IO.File.GetLastWriteTime(item.get_FileNames(1))
          };

          var projectFullName = item.ContainingProject.FullName;
          dirtyClass.AssemblyName = new AssemblyName(projectFullName);
          dirtyClasses.Add(dirtyClass);
        }
      }
      return dirtyClasses;
    }
  }
}
