using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.VisualStudio
{
  public class VsItem
  {
    public string Name { get; set; }
    public VsItemType Type { get; set; }
    public object Item { get; set; }

    public string Text
    {
      get
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        if (Item is null)
          return string.Empty;

        if (Item is ProjectItem projectItem)
          if (File.Exists(projectItem.FileNames[0]))
            return File.ReadAllText(projectItem.FileNames[0]);
        

        switch (Type)
        {
          case VsItemType.Solution:
            return "Solution";
          case VsItemType.SolutionFolder:
            return "Solution Folder";
          case VsItemType.Project:
            return "Project";
          case VsItemType.ProjectFolder:
            return "Project Folder";
          case VsItemType.ProjectProperties:
            return "Project Properties";
          case VsItemType.CSharpFile:
            var path = ((ProjectItem)Item).FileNames[0];
            return File.ReadAllText(path);
          case VsItemType.XamlFile:
            return "XAML File";
          case VsItemType.CodeBehindCSharp:
            return "Code Behind C#";
          case VsItemType.ProjectDependencies:
            return "Project Dependencies";
          case VsItemType.ProjectReferences:
            return "Project References";
          default:
            return string.Empty;
        }
      }
    }

  }
}
