using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

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


    //public static CodeElement GetSelectedClass(this DTE dte)
    //{
    //  Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
    //  UIHierarchy solutionExplorerHierarchy = (UIHierarchy)solutionExplorer.Object;
    //  Array selectedItems = (Array)solutionExplorerHierarchy.SelectedItems;

    //  foreach (UIHierarchyItem selectedItem in selectedItems)
    //  {
    //    ProjectItem projectItem = selectedItem.Object as ProjectItem;
    //    if (projectItem != null)
    //    {
    //      var prName = projectItem.Name;
    //      foreach (CodeElement codeElement in projectItem.FileCodeModel.CodeElements)
    //      {
    //        if (codeElement.Kind == vsCMElement.vsCMElementClass)
    //        {
    //          return codeElement;
    //        }
    //      }
    //    }
    //  }

    //  return null;
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
  }


}
