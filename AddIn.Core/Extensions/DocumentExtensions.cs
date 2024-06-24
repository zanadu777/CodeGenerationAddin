using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace AddIn.Core.Extensions
{
  public static class DocumentExtensions
  {
    public static CodeElementLocation CodeElementLocation(this Document document)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (document == null || document.ProjectItem == null || document.ProjectItem.ContainingProject == null)
        throw new ArgumentNullException("Document or its project item is null.");

      var projectItemLocation = new CodeElementLocation
      {
        ProjectName = document.ProjectItem.ContainingProject.Name,
        SolutionRelativePath = GetSolutionRelativePath(document.ProjectItem)
      };

      var codeElement = FindFirstSignificantCodeElement(document.ProjectItem.FileCodeModel.CodeElements);
      if (codeElement != null)
      {
        projectItemLocation.TypeName = codeElement.Name;
        projectItemLocation.NameSpace = (codeElement.Kind == vsCMElement.vsCMElementClass && ((CodeClass)codeElement).Namespace != null) ? ((CodeClass)codeElement).Namespace.FullName : string.Empty;
        // Trim off the 'vsCMElement' part for better JSON readability
        projectItemLocation.ElementType = codeElement.Kind.ToString().Replace("vsCMElement", "");
      }

      return projectItemLocation;
    }

    private static string GetSolutionRelativePath(ProjectItem projectItem)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      string fullPath = projectItem.Properties.Item("FullPath").Value.ToString();
      string solutionDir = Path.GetDirectoryName(projectItem.DTE.Solution.FullName);
      Uri solutionUri = new Uri(solutionDir + Path.DirectorySeparatorChar);
      Uri fullPathUri = new Uri(fullPath);

      return solutionUri.MakeRelativeUri(fullPathUri).ToString();
    }

    private static CodeElement FindFirstSignificantCodeElement(CodeElements elements)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      foreach (CodeElement element in elements)
      {
        if (element is CodeClass || element is CodeInterface || element is CodeStruct || element is CodeEnum)
          return element;

        if (element is CodeNamespace codeNamespace)
        {
          var nestedElement = FindFirstSignificantCodeElement(codeNamespace.Members);
          if (nestedElement != null)
            return nestedElement;
        }
      }
      return null;
    }
  }
}
