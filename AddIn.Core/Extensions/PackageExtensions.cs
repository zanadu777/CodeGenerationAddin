using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Extensions
{
  public static  class PackageExtensions
  {
    public static async Task<DTE2> GetDTE2Async(this AsyncPackage package)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
      var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
      return dte;
    }

    public static DTE2 GetDTE2Sync(this AsyncPackage package)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
      return dte;
    }
  }
}
