using System;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace CodeAddIn.VisualStudio
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

    public static VSItem SelectedItem
    {
      get
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        Window solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
        UIHierarchy solutionExplorerHierarchy = (UIHierarchy)solutionExplorer.Object;
        Array selectedItems = (Array)solutionExplorerHierarchy.SelectedItems;
        UIHierarchyItem selectedItem = selectedItems.Length > 0 ? (UIHierarchyItem)selectedItems.GetValue(0) : null;

        return FromUIHierarchyItem(selectedItem);
      }
    }





    public static VSItem FromUIHierarchyItem(UIHierarchyItem hItem)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (hItem is null)
        return new VSItem { Name = string.Empty, Type = VSItemType.Unknown, Item = null };

      if (hItem.Object is Solution solution)
        return new VSItem { Name = solution.FullName, Type = VSItemType.Solution, Item = solution };

      if (hItem.Object is Project project)
      {
        if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
          return new VSItem { Name = project.Name, Type = VSItemType.SolutionFolder, Item = project };

        return new VSItem { Name = project.Name, Type = VSItemType.Project, Item = project };
      }

      if (hItem.Object is ProjectItem projectItem)
      {
        if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFolder)
        {
          if (projectItem.Name == "Properties")
            return new VSItem { Name = projectItem.Name, Type = VSItemType.ProjectProperties, Item = projectItem };

          return new VSItem { Name = projectItem.Name, Type = VSItemType.ProjectFolder, Item = projectItem };
        }

        if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFile)
        {
          if (projectItem.Collection.Parent is ProjectItem parentItem)
          {
            if (projectItem.Name.EndsWith(".xaml.cs"))
              return new VSItem { Name = projectItem.Name, Type = VSItemType.CodeBehindCSharp, Item = projectItem };
          }

          if (projectItem.Name.EndsWith(".cs"))
            return new VSItem { Name = projectItem.Name, Type = VSItemType.CSharpFile, Item = projectItem };

          if (projectItem.Name.EndsWith(".xaml"))
            return new VSItem { Name = projectItem.Name, Type = VSItemType.XamlFile, Item = projectItem };
        }
      }

      if ((hItem.Name.Equals("Dependencies", StringComparison.OrdinalIgnoreCase) || hItem.Name.Equals("*Dependencies", StringComparison.OrdinalIgnoreCase)) && hItem.Object is object)
        return new VSItem { Name = hItem.Name, Type = VSItemType.ProjectDependencies, Item = hItem.Object };

      if (hItem.Name == "References" && hItem.Object is object)
        return new VSItem { Name = hItem.Name, Type = VSItemType.ProjectReferences, Item = hItem.Object };

      if (hItem.Object is VSLangProj.Reference reference)
        return new VSItem { Name = reference.Name, Type = VSItemType.Reference, Item = reference };

      return new VSItem { Name = hItem.Name, Type = VSItemType.Unknown, Item = hItem.Object };
    }


    public static SelectionEvents SelectionEvents => selectionEvents;
  }
}

