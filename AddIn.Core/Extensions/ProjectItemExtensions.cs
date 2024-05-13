using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AddIn.Core.Records;
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


    public static List<CodeElement> AllCodeElements(this ProjectItem projectItem)
    {
      if (projectItem.FileCodeModel == null)
      {
        return new List<CodeElement>(); 
      }

      var result = new List<CodeElement>();
      var stack = new Stack<CodeElements>();
      stack.Push(projectItem.FileCodeModel.CodeElements);

      while (stack.Count > 0)
      {
        CodeElements elements = stack.Pop();
        foreach (CodeElement element in elements)
        {
          result.Add(element);

          if (element.Children != null)
          {
            stack.Push(element.Children);
          }
        }
      }

      return result;
    }

    public static IEnumerable<ProjectItem> WhereText(this IEnumerable<ProjectItem> projectItems, Func<string, bool> filter)
    {
      return projectItems.Where(item =>
      {
        bool result = false;
        if (item.FileCount > 0)
        {
          string filePath = item.FileNames[0];
          if (File.Exists(filePath))
          {
            string content = File.ReadAllText(filePath);
            result = filter(content);
          }
        }
        return result;
      });
    }

    public static IEnumerable<SearchResult> GetSearchResultsCaseSensitive(this IEnumerable<ProjectItem> projectItems, string searchText)
    {
      foreach (var item in projectItems)
      {
        if (item.FileCount > 0)
        {
          string filePath = item.FileNames[0];
          if (File.Exists(filePath))
          {
            int lineNum = 1;
            foreach (var line in File.ReadLines(filePath))
            {
              if (line.Contains(searchText))
              {
                yield return new SearchResult
                {
                  Code = line,
                  File = Path.GetFileName(filePath),
                  Line = lineNum,
                  Col = line.IndexOf(searchText),
                  Path = filePath,
                  Extension = Path.GetExtension(filePath),
                  Project = item.ContainingProject.Name
                };
              }
              lineNum++;
            }
          }
        }
      }
    }


    public static IEnumerable<SearchResult> GetSearchResultsCaseInsensitive(this IEnumerable<ProjectItem> projectItems, string searchText)
    {
      foreach (var item in projectItems)
      {
        if (item.FileCount > 0)
        {
          string filePath = item.FileNames[0];
          if (File.Exists(filePath))
          {
            int lineNum = 1;
            foreach (var line in File.ReadLines(filePath))
            {
              if (line.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
              {
                yield return new SearchResult
                {
                  Code = line,
                  File = Path.GetFileName(filePath),
                  Line = lineNum,
                  Col = line.IndexOf(searchText, StringComparison.OrdinalIgnoreCase),
                  Path = filePath,
                  Extension = Path.GetExtension(filePath),
                  Project = item.ContainingProject.Name
                };
              }
              lineNum++;
            }
          }
        }
      }
    }

    public static IEnumerable<SearchResult> GetSearchResultsRegex(this IEnumerable<ProjectItem> projectItems, string regexPattern)
    {
      var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
      foreach (var item in projectItems)
      {
        if (item.FileCount > 0)
        {
          string filePath = item.FileNames[0];
          if (File.Exists(filePath))
          {
            string content = File.ReadAllText(filePath);
            var matches = regex.Matches(content);
            foreach (Match match in matches)
            {
              int lineNum = content.Take(match.Index).Count(c => c == '\n') + 1;
              int lastNewLine = content.LastIndexOf('\n', match.Index);
              int col = match.Index - (lastNewLine == -1 ? 0 : lastNewLine + 1);
              yield return new SearchResult
              {
                Code = match.Value,
                File = Path.GetFileName(filePath),
                Line = lineNum,
                Col = col,
                Path = filePath,
                Extension = Path.GetExtension(filePath),
                Project = item.ContainingProject.Name
              };
            }
          }
        }
      }
    }


    public static IEnumerable<SearchResult> GetSearchResultsWholeWord(this IEnumerable<ProjectItem> projectItems, string searchText, bool isCaseSensitive = false)
    {
      var regexOptions = isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
      var regex = new Regex($@"\b{Regex.Escape(searchText)}\b", regexOptions);
      foreach (var item in projectItems)
      {
        if (item.FileCount > 0)
        {
          string filePath = item.FileNames[0];
          if (File.Exists(filePath))
          {
            string content = File.ReadAllText(filePath);
            var matches = regex.Matches(content);
            foreach (Match match in matches)
            {
              int lineNum = content.Take(match.Index).Count(c => c == '\n') + 1;
              int lastNewLine = content.LastIndexOf('\n', match.Index);
              int col = match.Index - (lastNewLine == -1 ? 0 : lastNewLine + 1);
              yield return new SearchResult
              {
                Code = match.Value,
                File = Path.GetFileName(filePath),
                Line = lineNum,
                Col = col,
                Path = filePath,
                Extension = Path.GetExtension(filePath),
                Project = item.ContainingProject.Name
              };
            }
          }
        }
      }
    }

  }
}
