using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Extensions
{
  public static class ProjectItemExtensions
  {

    public static List<CodeClass> CodeClasses(this IEnumerable<ProjectItem> projectItems)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<CodeClass> classes = new List<CodeClass>();
      foreach (var projectItem in projectItems)
        classes.AddRange(projectItem.CodeClasses());
    
      return classes;
    }

    public static List<CodeClass> CodeClasses(this ProjectItem projectItem)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<CodeClass> classes = new List<CodeClass>();

      if (projectItem.FileCodeModel != null)
      {
        foreach (CodeElement element in projectItem.FileCodeModel.CodeElements)
        {
          if (element.Kind == vsCMElement.vsCMElementNamespace)
          {
            foreach (CodeElement member in ((CodeNamespace)element).Members.Cast<CodeElement>())
            {
              if (member.Kind == vsCMElement.vsCMElementClass)
              {
                classes.Add((CodeClass)member);
              }
            }
          }
        }
      }

      return classes;
    }


    public static List<CodeInterface> CodeInterfaces(this IEnumerable<ProjectItem> projectItems)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<CodeInterface> interfaces = new List<CodeInterface>();
      foreach (var projectItem in projectItems)
        interfaces.AddRange(projectItem.CodeInterfaces());

      return interfaces;
    }

    public static List<CodeInterface> CodeInterfaces(this ProjectItem projectItem)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<CodeInterface> interfaces = new List<CodeInterface>();

      if (projectItem.FileCodeModel != null)
      {
        foreach (CodeElement element in projectItem.FileCodeModel.CodeElements)
        {
          if (element.Kind == vsCMElement.vsCMElementNamespace)
          {
            foreach (CodeElement member in ((CodeNamespace)element).Members.Cast<CodeElement>())
            {
              if (member.Kind == vsCMElement.vsCMElementInterface)
              {
                interfaces.Add((CodeInterface)member);
              }
            }
          }
        }
      }

      return interfaces;
    }


  }
}
