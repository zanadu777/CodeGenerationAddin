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
  }
}
