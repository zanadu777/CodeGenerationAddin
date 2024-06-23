using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace AddIn.Core.Extensions
{
  public static class DocumentExtensions
  {
    public static ProjectItemLocation  ProjectItemLocation(this Document document)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (document == null || document.ProjectItem == null || document.ProjectItem.ContainingProject == null)
        throw new ArgumentNullException("Document or its project item is null.");

      var projectItemLocation = new ProjectItemLocation
      {
        ProjectName = document.ProjectItem.ContainingProject.Name,
        SolutionRelativePath = GetSolutionRelativePath(document.ProjectItem)
      };

      CodeClass codeClass = FindFirstCodeClass(document.ProjectItem.FileCodeModel.CodeElements);
      if (codeClass != null)
      {
        projectItemLocation.TypeName = codeClass.Name;
        projectItemLocation.NameSpace = codeClass.Namespace?.FullName;
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

    private static CodeClass FindFirstCodeClass(CodeElements elements)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      foreach (CodeElement element in elements)
      {
        if (element is CodeClass codeClass)
        {
          return codeClass;
        }
        else if (element is CodeNamespace codeNamespace)
        {
          CodeClass nestedClass = FindFirstCodeClass(codeNamespace.Members);
          if (nestedClass != null)
          {
            return nestedClass;
          }
        }
      }
      return null;
    }
  }
}
