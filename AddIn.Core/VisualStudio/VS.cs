using System;
using System.Threading.Tasks;
using AddIn.Core.Extensions;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.VisualStudio
{
  public class VS
  {
    static DTE2 dte;
    static SelectionEvents selectionEvents;

    public static async Task InitializeAsync(AsyncPackage package)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
      selectionEvents = dte.Events.SelectionEvents;
    }

    public static ProjectItem GetSelectedProjectItem()
    {
      try
      {
        Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
        UIHierarchy solutionExplorerHierarchy = (UIHierarchy)solutionExplorer.Object;

        Array selectedItems = (Array)solutionExplorerHierarchy.SelectedItems;
        UIHierarchyItem selectedItem = selectedItems.Length > 0 ? (UIHierarchyItem)selectedItems.GetValue(0) : null;
        if (selectedItem?.Object is ProjectItem projectItem)
          return projectItem;
      }
      catch { }

      return null;
    }

    public static Project GetSelectedProject()
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
      UIHierarchy solutionExplorerHierarchy = (UIHierarchy)solutionExplorer.Object;

      Array selectedItems = (Array)solutionExplorerHierarchy.SelectedItems;
      UIHierarchyItem selectedItem = selectedItems.Length > 0 ? (UIHierarchyItem)selectedItems.GetValue(0) : null;
      if (selectedItem?.Object is Project project)
        return project;

      return null;
    }

    public static VsItem SelectedItem
    {
      get
      {
        try
        {
          ThreadHelper.ThrowIfNotOnUIThread();
          Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
          UIHierarchy solutionExplorerHierarchy = (UIHierarchy) solutionExplorer.Object;
          Array selectedItems = (Array) solutionExplorerHierarchy.SelectedItems;
          UIHierarchyItem selectedItem = selectedItems.Length > 0 ? (UIHierarchyItem) selectedItems.GetValue(0) : null;

          return selectedItem.ToVSItem();
        }
        catch
        {
          return new VsItem();
        }
      }
    }

    public static SelectionEvents SelectionEvents => selectionEvents;
  }
}

