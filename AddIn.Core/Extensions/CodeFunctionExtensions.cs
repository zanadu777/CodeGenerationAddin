using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Extensions
{
  public static class CodeFunctionExtensions
  {
    public static void AddToInterfaces(this CodeFunction codeFunction, IEnumerable<CodeInterface> codeInterfaces)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (codeFunction != null)
      {
        foreach (var codeInterface in codeInterfaces)
        {
          if (codeInterface != null)
          {
            codeInterface.AddFunction(codeFunction.Name, vsCMFunction.vsCMFunctionFunction, codeFunction.Type, -1, codeFunction.Access);
          }
        }
      }
    }

    public static string GetFunctionCall(this CodeFunction codeFunction)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (codeFunction != null)
      {
        var parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(p => p.Name).ToArray();
        return $"{codeFunction.Name}({string.Join(", ", parameters)})";
      }

      return string.Empty;
    }

  }
}
