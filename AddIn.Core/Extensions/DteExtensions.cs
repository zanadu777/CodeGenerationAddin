using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AddIn.Core.Records;
using EnvDTE;
using EnvDTE80;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AddIn.Core.Extensions
{
  public static class DteExtensions
  {
    public static Project GetSelectedProject(this DTE dte)
    {
      Project activeProject = null;

      Array activeSolutionProjects = (Array)dte.ActiveSolutionProjects;
      if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
      {
        activeProject = (Project)activeSolutionProjects.GetValue(0);
      }

      return activeProject;
    }

    public static IVsSolution IVsSolution(this DTE dte)
    {
      ServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
      IVsSolution solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
      return solution;
    }

    public static CodeClass GetClassAtCursor(this DTE dte)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      TextDocument textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
      EditPoint editPoint = textDoc.Selection.ActivePoint.CreateEditPoint();
      FileCodeModel fileCodeModel = dte.ActiveDocument.ProjectItem.FileCodeModel;

      CodeElement element = fileCodeModel.CodeElementFromPoint(editPoint, vsCMElement.vsCMElementClass);

      return element as CodeClass;
    }


    public static CodeFunction GetMethodAtCursor(this DTE dte)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      TextDocument textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
      EditPoint editPoint = textDoc.Selection.ActivePoint.CreateEditPoint();
      FileCodeModel fileCodeModel = dte.ActiveDocument.ProjectItem.FileCodeModel;

      CodeElement element = fileCodeModel.CodeElementFromPoint(editPoint, vsCMElement.vsCMElementFunction);

      return element as CodeFunction;
    }

    public static CodeClass2 GetSelectedClass(this DTE dte)
    {
      Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
      UIHierarchy solutionExplorerHierarchy = (UIHierarchy)solutionExplorer.Object;
      Array selectedItems = (Array)solutionExplorerHierarchy.SelectedItems;

      foreach (UIHierarchyItem selectedItem in selectedItems)
      {
        ProjectItem projectItem = selectedItem.Object as ProjectItem;
        if (projectItem != null)
        {
          Stack<CodeElements> stack = new Stack<CodeElements>();
          stack.Push(projectItem.FileCodeModel.CodeElements);

          while (stack.Count > 0)
          {
            CodeElements codeElements = stack.Pop();
            foreach (CodeElement codeElement in codeElements)
            {
              if (codeElement.Kind == vsCMElement.vsCMElementClass)
              {
                return (CodeClass2)codeElement;
              }

              if (codeElement.Children.Count > 0)
              {
                stack.Push(codeElement.Children);
              }
            }
          }
        }
      }

      return null;
    }

    public static ProjectItem GetSelectedProjectItem(this DTE dte)
    {
      foreach (SelectedItem selectedItem in dte.SelectedItems)
      {
        ProjectItem projectItem = selectedItem.ProjectItem;
        if (projectItem != null)
        {
          return projectItem;
        }
      }

      return null;
    }

    public static Type GetSelectedType(this DTE dte)
    {
      var selectedCodeElement = dte.GetSelectedClass();

      var selectedProject = dte.GetSelectedProject();

      var selectedAssembly = selectedProject.ProjectAssembly();

      var selectedType = selectedAssembly.GetType(selectedCodeElement.FullName);

      return selectedType;
    }




    public static void SetBreakpointsOnMethods(this DTE2 dte, Document document)
    {
      if (document != null)
      {
        TextDocument textDoc = document.Object("TextDocument") as TextDocument;
        if (textDoc != null)
        {
          FileCodeModel fileCodeModel = document.ProjectItem.FileCodeModel;

          var codeElements = document.ProjectItem.AllCodeElements();

          List<CodeFunction> methods = codeElements.OfType<CodeFunction>().ToList();
          foreach (var method in methods)
            dte.SetBreakpointAtFunctionStart(method);
        }
      }
    }

    public static void SetBreakpointAtFunctionStart(this DTE2 dte, CodeFunction codeFunction)
    {
      if (codeFunction != null)
      {
        TextPoint startPoint = codeFunction.StartPoint;
        if (startPoint != null)
        {
          // Check if there is already a breakpoint at this location
          foreach (Breakpoint breakpoint in dte.Debugger.Breakpoints)
          {
            if (breakpoint.File == startPoint.Parent.Parent.FullName && breakpoint.FileLine == startPoint.Line)
            {
              // There is already a breakpoint at this location, so return without adding a new one
              return;
            }
          }

          // There is no breakpoint at this location, so add a new one
          dte.Debugger.Breakpoints.Add("", startPoint.Parent.Parent.FullName, startPoint.Line);
        }
      }
    }


    public static void RemoveAllBreakpoints(this DTE dte, ProjectItem projectItem)
    {
      string filePath = projectItem.FileNames[0];

      for (int i = dte.Debugger.Breakpoints.Count; i >= 1; i--)
      {
        Breakpoint breakpoint = dte.Debugger.Breakpoints.Item(i);
        if (breakpoint.File == filePath)
        {
          breakpoint.Delete();
        }
      }
    }

    public static string SelectedText(this DTE2 dte)
    {
      if (dte.ActiveDocument == null)
        return string.Empty;

      TextDocument textDoc = (TextDocument)dte.ActiveDocument.Object("TextDocument");
      EditPoint start = textDoc.Selection.TopPoint.CreateEditPoint();
      EditPoint end = textDoc.Selection.BottomPoint.CreateEditPoint();
      return start.GetText(end);
    }

    public static void GoTo(this DTE dte, SearchResult searchResult)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var document = dte.ItemOperations.OpenFile(searchResult.Path, EnvDTE.Constants.vsViewKindTextView);
      document.Activate();
      dte.ExecuteCommand("Edit.GoTo", searchResult.Line.ToString());

      var selection = (TextSelection)dte.ActiveDocument.Selection;
      selection.MoveToLineAndOffset(searchResult.Line, searchResult.Col);
      selection.EndOfLine(true);
    }


    public static IEnumerable<ProjectItem> GetOpenProjectItems(this DTE dte)
    {
      return dte.Windows
        .OfType<Window>()
        .Where(window => window.Type == vsWindowType.vsWindowTypeDocument)
        .Select(window => window.ProjectItem);
    }
  }
}
