using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AddIn.Core.Extensions
{
  public static class IVsHierarchyExtensions
  {
    public static List<ProjectItem> AllProjectItems(this IVsHierarchy projectHierarchy)
    {
      if (!projectHierarchy.IsProject())
        return new List<ProjectItem>();

      List<ProjectItem> projectItems = new List<ProjectItem>();
      Stack<uint> stack = new Stack<uint>();
      stack.Push(VSConstants.VSITEMID_ROOT);

      while (stack.Count > 0)
      {
        uint itemId = stack.Pop();

        // Skip the root item
        if (itemId != VSConstants.VSITEMID_ROOT)
        {
          ProjectItem projectItem = GetProjectItem(projectHierarchy, itemId);
          if (projectItem != null)
          {
            projectItems.Add(projectItem);
          }
        }

        object pVar;
        projectHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_FirstChild, out pVar);
        uint childId = GetItemId(pVar);

        while (childId != VSConstants.VSITEMID_NIL)
        {
          stack.Push(childId);
          projectHierarchy.GetProperty(childId, (int)__VSHPROPID.VSHPROPID_NextSibling, out pVar);
          childId = GetItemId(pVar);
        }
      }

      return projectItems;
    }

    public static List<ProjectItem> ProjectItems(this IVsHierarchy projectHierarchy, Predicate<ProjectItem> filter)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (projectHierarchy == null || filter == null)
        return new List<ProjectItem>();

      var allProjectItems = projectHierarchy.AllProjectItems();
      var filteredProjectItems = allProjectItems.Where(pi => filter(pi)).ToList();

      return filteredProjectItems;
    }


    public static List<CompleteCodeClass> AllCompleteCodeClasses(this IVsHierarchy projectHierarchy)
    {
      if (!projectHierarchy.IsProject())
        return new List<CompleteCodeClass>();

      var projectItems = projectHierarchy.AllProjectItems();
      var codeClasses = projectItems.CodeClasses();
      var groupedCodeClasses = codeClasses.GroupByFullName();
      var completeCodeClasses = groupedCodeClasses.Select(g => new CompleteCodeClass( g.Value)).ToList();
      return completeCodeClasses;
    }

    public static List<CompleteCodeInterface> AllCompleteCodeInterfaces(this IVsHierarchy projectHierarchy)
    {
      if (!projectHierarchy.IsProject())
        return new List<CompleteCodeInterface>();

      var projectItems = projectHierarchy.AllProjectItems();
      var codeInterfaces = projectItems.CodeInterfaces();
      var groupedCodeInterfaces = codeInterfaces.GroupByFullName();
      var completeCodeInterfaces = groupedCodeInterfaces.Select(g => new CompleteCodeInterface(g.Value)).ToList();
      return completeCodeInterfaces;
    }


    private static uint GetItemId(object pvar)
    {
      if (pvar == null) return VSConstants.VSITEMID_NIL;
      if (pvar is int) return (uint)(int)pvar;
      if (pvar is uint) return (uint)pvar;
      return VSConstants.VSITEMID_NIL;
    }

    private static ProjectItem GetProjectItem(IVsHierarchy hierarchy, uint itemId)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out var pVar);
      return pVar as ProjectItem;
    }






    public static Type GetExtObjectType(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint) VSConstants.VSITEMID.Root, (int) __VSHPROPID.VSHPROPID_ExtObject,
        out object extObject);

      return extObject?.GetType();
    }

    public static string GetProjectName(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      int name = (int)__VSHPROPID.VSHPROPID_Name;
      hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, name, out object projectName);

      return projectName?.ToString();
    }



    public static IEnumerable<IVsHierarchy> GetAllDescendants(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      Queue<IVsHierarchy> queue = new Queue<IVsHierarchy>();
      int firstChild = (int)__VSHPROPID.VSHPROPID_FirstChild;
      int nextSibling = (int)__VSHPROPID.VSHPROPID_NextSibling;

      hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, firstChild, out object childId);

      while ((uint)childId != VSConstants.VSITEMID_NIL)
      {
        hierarchy.GetProperty((uint)childId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object childObject);
        if (childObject is IVsHierarchy childHierarchy)
        {
          queue.Enqueue(childHierarchy);
        }

        hierarchy.GetProperty((uint)childId, nextSibling, out childId);
      }

      while (queue.Count > 0)
      {
        IVsHierarchy current = queue.Dequeue();
        yield return current;

        current.GetProperty(VSConstants.VSITEMID_ROOT, firstChild, out childId);
        while ((uint)childId != VSConstants.VSITEMID_NIL)
        {
          current.GetProperty((uint)childId, nextSibling, out childId);
        }
      }
    }


    public static IEnumerable<KeyValuePair<uint, IVsHierarchy>> GetAllDescendantPairs(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      Queue<KeyValuePair<uint, IVsHierarchy>> queue = new Queue<KeyValuePair<uint, IVsHierarchy>>();
      int firstChild = (int)__VSHPROPID.VSHPROPID_FirstChild;
      int nextSibling = (int)__VSHPROPID.VSHPROPID_NextSibling;

      hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, firstChild, out object childIdObj);
      if (childIdObj is int childId)
      {
        while (!IsNil(childIdObj))
        {
          hierarchy.GetProperty((uint)childId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object childObject);
          if (childObject is IVsHierarchy childHierarchy)
          {
            queue.Enqueue(new KeyValuePair<uint, IVsHierarchy>((uint)childId, childHierarchy));
          }

          hierarchy.GetProperty((uint)childId, nextSibling, out object nextChildIdObj);
          if (nextChildIdObj is int nextChildId)
          {
            childId = nextChildId;
          }
          else
          {
            // Handle the case where nextChildIdObj is not an int
            break;
          }
        }

        while (queue.Count > 0)
        {
          KeyValuePair<uint, IVsHierarchy> current = queue.Dequeue();
          yield return current;

          current.Value.GetProperty(VSConstants.VSITEMID_ROOT, firstChild, out childIdObj);
          if (childIdObj is int rootChildId)
          {
            childId = rootChildId;
            while (!IsNil(childIdObj))
            {
              current.Value.GetProperty((uint)childId, nextSibling, out childIdObj);
              if (childIdObj is int siblingChildId)
              {
                childId = siblingChildId;
              }
              else
              {
                // Handle the case where childIdObj is not an int
                break;
              }
            }
          }
        }
      }
    }


  

    private static bool IsNil(object childIdObj)
    {
      if (childIdObj is int childId)
      {
        return childId == unchecked((int)VSConstants.VSITEMID_NIL);
      }
      else if (childIdObj is uint uintChildId)
      {
        return uintChildId == VSConstants.VSITEMID_NIL;
      }
      else
      {
        // Handle the case where childIdObj is not an int or uint
        return false;
      }
    }

    public static IEnumerable<string> GetAllNames(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      foreach (var pair in hierarchy.GetAllDescendantPairs())
      {
        string name = pair.GetName();
        if (name != null)
        {
          yield return name;
        }
      }
    }


    public static string GetName(this KeyValuePair<uint, IVsHierarchy> pair)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      int namePropId = (int)__VSHPROPID.VSHPROPID_Name;
      pair.Value.GetProperty(pair.Key, namePropId, out object name);

      return name?.ToString();
    }

    public static string GetFileName(this KeyValuePair<uint, IVsHierarchy> pair)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      int fileNamePropId = (int)__VSHPROPID.VSHPROPID_SaveName;
      pair.Value.GetProperty(pair.Key, fileNamePropId, out object fileName);

      return fileName != null ? fileName.ToString() : null;
    }

    public static List<string> GetCSharpFiles(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var project = hierarchy.ToProject();
      if (project == null)
        return new List<string>();

      var csharpFiles = new List<string>();
      foreach (EnvDTE.ProjectItem item in project.ProjectItems)
      {
        AddCSharpFiles(item, csharpFiles);
      }

      return csharpFiles;
    }

    public static EnvDTE.Project ToProject(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject);

      return extObject as EnvDTE.Project;
    }


    private static void AddCSharpFiles(EnvDTE.ProjectItem item, List<string> csharpFiles)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      for (short i = 0; i < item.FileCount; i++)
      {
        string filePath = item.FileNames[i];
        if (filePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
          csharpFiles.Add(filePath);
        }
      }

      foreach (EnvDTE.ProjectItem childItem in item.ProjectItems)
      {
        AddCSharpFiles(childItem, csharpFiles);
      }
    }


    


    public static IEnumerable<IVsHierarchy> GetAllProjects(this IVsSolution solution)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      Guid guid = Guid.Empty;
      solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out IEnumHierarchies enumHierarchies);

      IVsHierarchy[] hierarchy = new IVsHierarchy[1];
      while (enumHierarchies.Next(1, hierarchy, out uint fetched) == VSConstants.S_OK && fetched == 1)
      {
        yield return hierarchy[0];
      }
    }


    public static bool IsSolutionFolder(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetGuidProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_TypeGuid, out Guid projectType);
      return projectType == new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}"); // GUID for Solution Folder
    }


    public static bool HasPhysicalLocation(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ProjectDir, out object projectDir);
      return !string.IsNullOrEmpty(projectDir?.ToString());
    }

 
    public static bool IsProject(this IVsHierarchy hierarchy)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      int typeGuidPropId = (int)__VSHPROPID.VSHPROPID_TypeGuid;

      List<Guid> projectTypeGuids = new List<Guid>
      {
        new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"), // C# Project
        new Guid("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"), // VB.NET Project
        new Guid("{F2A71F9B-5D33-465A-A702-920D77279786}"), // F# Project
        new Guid("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"), // C++ Project
      };

      hierarchy.GetGuidProperty((uint)VSConstants.VSITEMID.Root, typeGuidPropId, out Guid typeGuid);
      return projectTypeGuids.Contains(typeGuid);
    }
  }
}