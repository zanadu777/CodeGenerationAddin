using CodeModel;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Extensions
{
  public static class SolutionExtensions
  {
    public static void BuildIfDirty(this Solution solution)
    {
      try
      {
        if (solution.SolutionBuild.LastBuildInfo != 0)
        {
          solution.SolutionBuild.Build(true);
        }
      }
      catch (System.Runtime.InteropServices.COMException)
      {
        solution.SolutionBuild.Build(true);
      }
    }

    public static List<Project> Projects(this Solution solution, EProjectFilter filter)
    {
      foreach (Project project in solution.Projects)
        Debug.WriteLine($"{project.Name} {project.Kind}");

      List<Project> projects = new List<Project>();
      foreach (Project project in solution.Projects)
      {
        if (project.Kind != null)
        {
          if (filter == EProjectFilter.ExcludeVsProjectKindMisc)
          {
            if (project.Kind  != "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}" && project.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
            {
              var name = project.Name;
              Debug.WriteLine($"{project.Name} {project.Kind}");
              projects.Add(project);
            }
          }
        }
      }
      return projects;
    }

    public static List<DirtyClass> DirtyClasses(this EnvDTE.Solution solution)
    {
      List<DirtyClass> dirtyClasses = new List<DirtyClass>();
      foreach (EnvDTE.Project project in solution.Projects(EProjectFilter.ExcludeVsProjectKindMisc))
        dirtyClasses.AddRange(project.DirtyClasses());

      return dirtyClasses;
    }
  }
}
