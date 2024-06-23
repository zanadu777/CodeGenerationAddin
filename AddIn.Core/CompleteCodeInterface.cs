using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core
{
  public class CompleteCodeInterface
  {
    public List<CodeInterface> CodeInterfaces {get; set; } = new List<CodeInterface>();

    public CompleteCodeInterface(CodeInterface codeInterface)
    {
      CodeInterfaces.Add(codeInterface);
    }

    public CompleteCodeInterface(IEnumerable<CodeInterface> codeInterfaces)
    {
      CodeInterfaces.AddRange(codeInterfaces);
    }

    public List<CodeInterface> WithFileName(string fileName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      return CodeInterfaces.Where(c => c.ProjectItem.FileNames[0] == fileName).ToList();
    }
  }
}
