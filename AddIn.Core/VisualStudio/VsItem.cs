using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AddIn.Core.VisualStudio
{
  public class VsItem
  {
    public string Name { get; set; }
    public VSItemType Type { get; set; }
    public object Item { get; set; }

    public string Text
    {
      get
      {
        if (Item is null)
          return string.Empty;

        if (Item is ProjectItem projectItem)
          if (File.Exists(projectItem.FileNames[0]))
            return File.ReadAllText(projectItem.FileNames[0]);
        

        switch (Type)
        {
          case VSItemType.Solution:
            return "Solution";
          case VSItemType.SolutionFolder:
            return "Solution Folder";
          case VSItemType.Project:
            return "Project";
          case VSItemType.ProjectFolder:
            return "Project Folder";
          case VSItemType.ProjectProperties:
            return "Project Properties";
          case VSItemType.CSharpFile:
            var path = ((ProjectItem)Item).FileNames[0];
            return File.ReadAllText(path);
          case VSItemType.XamlFile:
            return "XAML File";
          case VSItemType.CodeBehindCSharp:
            return "Code Behind C#";
          case VSItemType.ProjectDependencies:
            return "Project Dependencies";
          case VSItemType.ProjectReferences:
            return "Project References";
          default:
            return string.Empty;
        }
        return string.Empty;
      }
    }

  }
}
