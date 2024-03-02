using CodeModel;
using EnvDTE;
using System;
using System.Collections.Generic;
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

    public static List<DirtyClass> DirtyClasses(this EnvDTE.Solution solution)
    {
      List<DirtyClass> dirtyClasses = new List<DirtyClass>();
      foreach (EnvDTE.Project project in solution.Projects)
        dirtyClasses.AddRange(project.DirtyClasses());

      return dirtyClasses;
    }

  }
}
