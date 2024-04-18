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

        if (codeInterface != null && codeFunction != null)
        {
          var parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(p => p.Type.AsString).ToArray();
          codeInterface.AddFunction(codeFunction.Name, vsCMFunction.vsCMFunctionFunction, codeFunction.Type, -1, codeFunction.Access);
        }
      }
    }
    
}
