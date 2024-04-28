using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.Extensions
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

    public static void OpenProjectItem(this ProjectItem projectItem, bool isScrolledToBottom)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (projectItem == null)
        return;

      // Open the project item
      Window window = projectItem.Open(Constants.vsViewKindCode);
      window.Visible = true;
      window.Activate();

      if (isScrolledToBottom && projectItem.Document != null)
      {
        // Get the TextDocument object
        TextDocument textDoc = (TextDocument)projectItem.Document.Object("TextDocument");

        // Get the EditPoint at the end of the document
        EditPoint endPoint = textDoc.EndPoint.CreateEditPoint();

        // Scroll to the end of the document
        textDoc.Selection.MoveToPoint(endPoint, false);
      }
    }

    public static void TransformText(this ProjectItem projectItem, Func<string, string> transform)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (projectItem == null || projectItem.Document == null)
        return;

      // Get the TextDocument object
      TextDocument textDoc = (TextDocument)projectItem.Document.Object("TextDocument");

      // Get the text of the document
      string text = textDoc.StartPoint.CreateEditPoint().GetText(textDoc.EndPoint);

      // Apply the transformation
      string modifiedText = transform(text);

      // Check if the text was modified
      if (text != modifiedText)
      {
        // Start an undo context
        projectItem.DTE.UndoContext.Open("Transform text");

        try
        {
          // Replace the text of the document
          textDoc.StartPoint.CreateEditPoint().ReplaceText(textDoc.EndPoint, modifiedText, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
        }
        finally
        {
          // Close the undo context
          projectItem.DTE.UndoContext.Close();
        }
      }
    }

    public static string RemoveExtraBlankLines(string text)
    {
      // Replace multiple blank lines with a single blank line
      return Regex.Replace(text, @"(\r?\n)\s*\1", "$1$1");
    }
  }
}
