﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core.VisualStudio;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.Extensions
{
  public static class UIHierarchyItemExtensions
  {
    public static VsItem ToVSItem(this UIHierarchyItem hItem)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (hItem is null)
        return new VsItem { Name = string.Empty, Type = VSItemType.Unknown, Item = null };

      if (hItem.Object is Solution solution)
        return new VsItem { Name = solution.FullName, Type = VSItemType.Solution, Item = solution };

      if (hItem.Object is Project project)
      {
        if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
          return new VsItem { Name = project.Name, Type = VSItemType.SolutionFolder, Item = project };

        return new VsItem { Name = project.Name, Type = VSItemType.Project, Item = project };
      }

      if (hItem.Object is ProjectItem projectItem)
      {
        if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFolder)
        {
          if (projectItem.Name == "Properties")
            return new VsItem { Name = projectItem.Name, Type = VSItemType.ProjectProperties, Item = projectItem };

          return new VsItem { Name = projectItem.Name, Type = VSItemType.ProjectFolder, Item = projectItem };
        }

        if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFile)
        {
          if (projectItem.Collection.Parent is ProjectItem parentItem)
          {
            if (projectItem.Name.EndsWith(".xaml.cs"))
              return new VsItem { Name = projectItem.Name, Type = VSItemType.CodeBehindCSharp, Item = projectItem };
          }

          if (projectItem.Name.EndsWith(".cs"))
            return new VsItem { Name = projectItem.Name, Type = VSItemType.CSharpFile, Item = projectItem };

          if (projectItem.Name.EndsWith(".xaml"))
            return new VsItem { Name = projectItem.Name, Type = VSItemType.XamlFile, Item = projectItem };
        }
      }
      if ((hItem.Name.Equals("Dependencies", StringComparison.OrdinalIgnoreCase) || hItem.Name.Equals("*Dependencies", StringComparison.OrdinalIgnoreCase)) && hItem.Object is object)
        return new VsItem { Name = hItem.Name, Type = VSItemType.ProjectDependencies, Item = hItem.Object };
      if (hItem.Name == "References" && hItem.Object is object)
        return new VsItem { Name = hItem.Name, Type = VSItemType.ProjectReferences, Item = hItem.Object };

      if (hItem.Object is VSLangProj.Reference reference)
        return new VsItem { Name = reference.Name, Type = VSItemType.Reference, Item = reference };

      return new VsItem { Name = hItem.Name, Type = VSItemType.Unknown, Item = hItem.Object };
    }
  }
}
