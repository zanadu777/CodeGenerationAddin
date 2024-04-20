using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Extensions
{
  public static class CodeInterfaceExtensions
  {

    public static void Add(this CodeInterface codeInterface, CodeFunction codeFunction)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (codeInterface == null || codeFunction == null || codeInterface.Contains(codeFunction))
        return;

      CodeFunction newFunction = codeInterface.AddFunction(codeFunction.Name, vsCMFunction.vsCMFunctionFunction, codeFunction.Type, -1, codeFunction.Access);

      foreach (CodeParameter parameter in codeFunction.Parameters)
        newFunction.AddParameter(parameter.Name, parameter.Type, -1);
    }


    public static bool Contains(this CodeInterface codeInterface, CodeFunction codeFunction)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (codeInterface == null || codeFunction == null)
        return false;

      foreach (CodeElement member in codeInterface.Members)
      {
        if (member is CodeFunction existingFunction && existingFunction.Name == codeFunction.Name)
        {
          bool parametersMatch = existingFunction.Parameters.Cast<CodeParameter>().Select(p => p.Type.AsFullName).SequenceEqual(
            codeFunction.Parameters.Cast<CodeParameter>().Select(p => p.Type.AsFullName));

          if (parametersMatch)
            return true; // Function with the same signature exists
        }
      }

      return false; // No function with the same signature found
    }

  }

}
