using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Extensions
{
  public static  class CodeClassExtensions
  {
    public static Dictionary<string, List<CodeClass>> GroupByFullName(this IEnumerable<CodeClass> classes)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      Dictionary<string, List<CodeClass>> groupedClasses = new Dictionary<string, List<CodeClass>>();

      foreach (var codeClass in classes)
      {
        if (!groupedClasses.ContainsKey(codeClass.FullName))
        {
          groupedClasses[codeClass.FullName] = new List<CodeClass>();
        }

        groupedClasses[codeClass.FullName].Add(codeClass);
      }

      return groupedClasses;
    }

    public static Dictionary<string, List<CodeInterface>> GroupByFullName(this IEnumerable<CodeInterface> interfaces)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      Dictionary<string, List<CodeInterface>> groupedInterfaces = new Dictionary<string, List<CodeInterface>>();

      foreach (var codeInterface in interfaces)
      {
        if (!groupedInterfaces.ContainsKey(codeInterface.FullName))
        {
          groupedInterfaces[codeInterface.FullName] = new List<CodeInterface>();
        }

        groupedInterfaces[codeInterface.FullName].Add(codeInterface);
      }

      return groupedInterfaces;
    }

    public static void AddFunctionToClass(this CodeClass codeClass, CodeFunction codeFunction, string newBody)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (codeClass != null && codeFunction != null)
      {
        var parameters = codeFunction.Parameters.Cast<CodeParameter>().Select(p => p.Type.AsString).ToArray();
        var newFunction = codeClass.AddFunction(codeFunction.Name, vsCMFunction.vsCMFunctionFunction, codeFunction.Type, -1, codeFunction.Access);

        EditPoint start = newFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
        EditPoint end = newFunction.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
        start.ReplaceText(end, newBody, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
      }
    }


    public static List<CodeInterface> GetImplementedInterfaces(this IEnumerable<CodeClass> codeClasses)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      HashSet<CodeInterface> interfaces = new HashSet<CodeInterface>();

      foreach (var codeClass in codeClasses)
      {
        foreach (CodeElement element in codeClass.ImplementedInterfaces)
        {
          if (element is CodeInterface codeInterface)
          {
            interfaces.Add(codeInterface);
          }
        }
      }

      return interfaces.ToList();
    }
  }
}
