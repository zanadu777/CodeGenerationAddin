using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core.Extensions;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using CodeAddIn.Gui.InfoWindows;
using CodeAddIn.Gui;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace CodeAddIn.Lib
{
    public static class CodeAddInPackageProxy
    {
    public static Package Package { get; set; }

    public static void ProcessCode(object sender, EventArgs e)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
      var codeClass = dte.GetClassAtCursor();
      var method = dte.GetMethodAtCursor();

      var result = $"{codeClass.Name} {method.Name}";
    }


    public static void ExecuteInspectProject(object sender, EventArgs e)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
      Array projects = (Array)dte.ActiveSolutionProjects;
      Project selectedProject = (Project)projects.GetValue(0);

      var display = new CodeAddIn.Gui.InfoWindows.ProjectInfo();
      display.DataContext = new ProjectInfoVm { ProjectName = selectedProject.Name };
      display.Show();
    }

    public static void ExecuteInspectSolution(object sender, EventArgs e)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      StringBuilder sb = new StringBuilder();
      IVsSolution solution = Package.GetService<SVsSolution, IVsSolution>();
      var projects = solution.GetProjects();

      foreach (var project in projects)
      {
        var name = project.GetProjectName();
        sb.AppendLine(name);

        var items = project.AllProjectItems().ToList();
        var classes = project.AllCompleteCodeClasses().ToList();

        foreach (var item in items)
        {
          sb.AppendLine($"  {item.Name}");
          foreach (var codeClass in item.CodeClasses())
            sb.AppendLine($"    {codeClass.Name}");
        }

      }

      var display = new TextDisplay { Text = sb.ToString() };
      display.Show();
    }


    //public static void ExecuteShowModified(object sender, EventArgs e)
    //{

    //  var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)));
    //  var solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
    //  var allProjects = solution.GetAllProjects().ToList();
    //  var allNames = allProjects.Select(h => h.GetProjectName()).ToList();
    //  var filteredProjects = allProjects.Where(h => h.HasPhysicalLocation()).ToList();
    //  var filteredNames = filteredProjects.Select(h => h.GetProjectName()).ToList();
    //  //StringBuilder sb = new StringBuilder();
    //  //DTE dte = (DTE)GetService(typeof(DTE));
    //  //var solution = dte.Solution;
    //  //var dirty = solution.DirtyClasses();
    //  var window = Package.FindToolWindow(typeof(DirtyClassesToolWindow), 0, true) as DirtyClassesToolWindow;
    //  //if (null == window || null == window.Frame)
    //  //  throw new NotSupportedException("Cannot create tool window");

    //  //window.UpdateDirtyClasses(dirty);
    //  IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
    //  ErrorHandler.ThrowOnFailure(windowFrame.Show());
    //}


    public static void ExecuteCsharpInfo(object sender, EventArgs e)
    {
      //DTE dte = (DTE)GetService(typeof(DTE));
      //CodeClass2 selectedElement = dte.GetSelectedClass();
      //var factory = new CodeModelFactory();
      //var data = factory.CreateClassInfoData(selectedElement);
      ////var selectedType = dte.GetSelectedType();
      //var selectedProjectItem = dte.GetSelectedProjectItem();

      //if (selectedElement != null && selectedProjectItem != null )
      //{
      //  string className = selectedElement.Name;
      //  var csharpInfoVm = new CsharpInfoVm { ClassName = className};
      //  var display = new CsharpInfo {DataContext= csharpInfoVm };
      // display.Show();
      //}
      //else
      //{
      //  // No class is selected...
      //}
    }
  }
}
