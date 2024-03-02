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
  }
}
