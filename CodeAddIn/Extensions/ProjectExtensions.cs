using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeAddIn.Extensions
{
  public static class ProjectExtensions
  {
    public  static List<Project> ReferencedProjects(this Project project)
    {
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
        project.DTE.Solution.BuildIfDirty();

        string outputDir = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
        string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();
        string projectDir = System.IO.Path.GetDirectoryName(project.FullName);
        string assemblyPath = System.IO.Path.Combine(projectDir, outputDir, outputFileName);

        return Assembly.LoadFrom(assemblyPath);
      }

      public static List<ProjectItem> ProjectItemsWithMultipleFiles(this Project project)
      {
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


  }
}
