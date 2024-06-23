using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.Extensions
{
  public static class ProjectExtensions
  {
    public  static List<Project> ReferencedProjects(this Project project)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<Project> referencedProjects = new List<Project>();

      foreach (ProjectItem projectItem in project.ProjectItems)
      {
        if (projectItem.Object is VSLangProj.Reference reference)
        {
          if (reference.SourceProject != null)
          {
            referencedProjects.Add(reference.SourceProject);
          }
        }
      }

      return referencedProjects;
    }

    public static  List<string>  ReferencedNuGetPackages(this Project project)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<string> referencedPackages = new List<string>();
      string projectFilePath = project.FullName;
      XDocument projectFile = XDocument.Load(projectFilePath);
      var packageReferences = projectFile.Descendants("PackageReference");

      foreach (var packageReference in packageReferences)
      {
        string packageName = packageReference.Attribute("Include")?.Value;

        if (!string.IsNullOrEmpty(packageName))
        {
          referencedPackages.Add(packageName);
        }
      }

      return referencedPackages;
    }

      public static List<Type> TypesInProject(this Project project)
      {
        //project.DTE.Solution.BuildIfDirty();

        //string outputDir = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
        //string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();
        //string projectDir = System.IO.Path.GetDirectoryName(project.FullName);
        //string assemblyPath = System.IO.Path.Combine(projectDir, outputDir, outputFileName);

        Assembly assembly = project.ProjectAssembly();

        Type[] types = assembly.GetTypes();

        return new List<Type>(types);
      }

      public static Assembly ProjectAssembly(this Project project)
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        project.DTE.Solution.BuildIfDirty();

        string outputDir = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
        string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();
        string projectDir = System.IO.Path.GetDirectoryName(project.FullName);
        string assemblyPath = System.IO.Path.Combine(projectDir, outputDir, outputFileName);

        return Assembly.LoadFrom(assemblyPath);
      }


    #region Returning ProjectItems

    public static List<ProjectItem> ProjectItemsWithMultipleFiles(this Project project)
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        List<ProjectItem> projectItems = new List<ProjectItem>();

        foreach (ProjectItem projectItem in project.ProjectItems)
        {
          if (projectItem.FileCount > 1)
          {
            projectItems.Add(projectItem);
          }
        }

        return projectItems;
      }

      public static List<ProjectItem> ProjectItemsWithMultipleFiles(this Project project, Func<string, bool> filter)
      {
      ThreadHelper.ThrowIfNotOnUIThread();
      List<ProjectItem> projectItems = new List<ProjectItem>();

        foreach (ProjectItem projectItem in project.ProjectItems)
        {
          if (projectItem.FileCount > 1)
          {
            for (short i = 1; i <= projectItem.FileCount; i++)
            {
              string fileName = projectItem.FileNames[i];

              if (filter(fileName))
              {
                projectItems.Add(projectItem);
                break;
              }
            }
          }
        }

        return projectItems;
      }


    #endregion

    #region Returning ProjectItem

    public static ProjectItem ProjectItem(this Project project, string nameSpace, string typeName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      // Convert the namespace to a path based on the project's root namespace
      string rootNamespace = project.Properties.Item("DefaultNamespace").Value.ToString();
      string relativePath = nameSpace.StartsWith(rootNamespace) ? nameSpace.Substring(rootNamespace.Length).Replace('.', '\\') : nameSpace.Replace('.', '\\');
      string expectedPath = $"{relativePath}\\{typeName}.cs"; // Assuming C# source file

      // Try to find the item at the expected path
      try
      {
        ProjectItem item = project.ProjectItems.Item(expectedPath);
        if (item != null)
        {
          return item;
        }
      }
      catch (ArgumentException)
      {
        // Item not found at the expected path, continue to scan
      }

      // If not found, scan the entire project
      return ScanProjectItems(project.ProjectItems, typeName);
    }

    private static ProjectItem ScanProjectItems(ProjectItems items, string itemName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      foreach (ProjectItem item in items)
      {
        if (item.Name.Equals(itemName + ".cs", StringComparison.OrdinalIgnoreCase))
        {
          return item;
        }

        if (item.ProjectItems != null && item.ProjectItems.Count > 0)
        {
          ProjectItem foundItem = ScanProjectItems(item.ProjectItems, itemName);
          if (foundItem != null)
          {
            return foundItem;
          }
        }
      }

      return null;
    }


    #endregion




  }
}
