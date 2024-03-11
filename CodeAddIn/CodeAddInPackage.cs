using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CodeAddIn.Extensions;
using CodeAddIn.Gui;
using CodeAddIn.Gui.InfoWindows;
using CodeAddIn.Helpers;
using CodeModel;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio;
using EnvDTE;
using EnvDTE80;
using System.Linq;

namespace CodeAddIn
{
  /// <summary>
  /// This is the class that implements the package exposed by this assembly.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The minimum requirement for a class to be considered a valid package for Visual Studio
  /// is to implement the IVsPackage interface and register itself with the shell.
  /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
  /// to do it: it derives from the Package class that provides the implementation of the
  /// IVsPackage interface and uses the registration attributes defined in the framework to
  /// register itself and its components with the shell. These attributes tell the pkgdef creation
  /// utility what data to put into .pkgdef file.
  /// </para>
  /// <para>
  /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
  /// </para>
  /// </remarks>
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [Guid(CodeAddInPackage.PackageGuidString)]
  [ProvideMenuResource("Menus.ctmenu", 1)]
  [ProvideToolWindow(typeof(DirtyClassesToolWindow))]
  public sealed class CodeAddInPackage : AsyncPackage
  {
    /// <summary>
    /// CodeAddInPackage GUID string.
    /// </summary>
    public const string PackageGuidString = "0efaee20-08e5-4944-8ba7-a3eec5165f91";

    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initialization code that rely on services provided by VisualStudio.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
    /// <param name="progress">A provider for progress updates.</param>
    /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
      // When initialized asynchronously, the current thread may be a background thread at this point.
      // Do any initialization that requires the UI thread after switching to the UI thread.
      await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
      await Command1.InitializeAsync(this);

      var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
      commandService.AddCommand(PackageIds.CommandSolutionInfo, this.ExecuteSolutionInfo);
      commandService.AddCommand(PackageIds.CommandInspectSolution, this.ExecuteInspectSolution);
      commandService.AddCommand(PackageIds.ProjectInfoCommand, this.ExecuteInspectProject);
      commandService.AddCommand(PackageIds.CsharpInfoCommand, this.ExecuteCsharpInfo);
      commandService.AddCommand(PackageIds.CommandShowModified, this.ExecuteShowModified);
      //if (commandService != null)
      //{
      //  var cmdId = new CommandID(PackageGuids.CmdSet, (int)PackageIds.CommandInspectSolution);
      //  var menuItem = new MenuCommand(this.ExecuteInspectSolution, cmdId);
      //  commandService.AddCommand(menuItem);
      //}

      IsolatedStorageHelper.WriteToIsolatedStorage("key", "value");
      var val = IsolatedStorageHelper.ReadFromIsolatedStorage("key");
      await DirtyClassesToolWindowCommand.InitializeAsync(this);
        
    }

    private void ExecuteSolutionInfo(object sender, EventArgs e)
    {
       
    }

    private void ExecuteShowModified(object sender, EventArgs e)
    {
      var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)));
      var solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
      var allProjects = solution.GetAllProjects().ToList();
      var allNames = allProjects.Select(h => h.GetProjectName()).ToList();
      var filteredProjects = allProjects.Where(h =>   h.HasPhysicalLocation()).ToList();
      var filteredNames = filteredProjects.Select(h => h.GetProjectName()).ToList();
      //StringBuilder sb = new StringBuilder();
      //DTE dte = (DTE)GetService(typeof(DTE));
      //var solution = dte.Solution;
      //var dirty = solution.DirtyClasses();
      var window = this.FindToolWindow(typeof(DirtyClassesToolWindow), 0, true) as DirtyClassesToolWindow;
      //if (null == window || null == window.Frame)
      //  throw new NotSupportedException("Cannot create tool window");

      //window.UpdateDirtyClasses(dirty);
      IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
      ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }

    protected override void Dispose(bool disposing)
    {
      // This method is called when the package is being unloaded
      if (disposing)
      {
        // Add your cleanup code here
      }

      base.Dispose(disposing);
    }

    private void ExecuteCsharpInfo(object sender, EventArgs e)
    {
      DTE dte = (DTE)GetService(typeof(DTE));
      CodeClass2 selectedElement = dte.GetSelectedClass();
      var factory = new CodeModelFactory();
      var data = factory.CreateClassInfoData(selectedElement);
      //var selectedType = dte.GetSelectedType();
      var selectedProjectItem = dte.GetSelectedProjectItem();

      if (selectedElement != null && selectedProjectItem != null )
      {
        string className = selectedElement.Name;
        var csharpInfoVm = new CsharpInfoVm { ClassName = className};
        var display = new CsharpInfo {DataContext= csharpInfoVm };
       display.Show();
      }
      else
      {
        // No class is selected...
      }
    }

    private void ExecuteInspectProject(object sender, EventArgs e)
    {
      DTE dte = (DTE)GetService(typeof(DTE));
      Array projects = (Array)dte.ActiveSolutionProjects;
      Project selectedProject = (Project)projects.GetValue(0);

      var display = new ProjectInfo();
      display.DataContext = new ProjectInfoVm{ ProjectName= selectedProject.Name };
      display.Show();
    }

    private void ExecuteInspectSolution(object sender, EventArgs e)
    {
      StringBuilder sb = new StringBuilder();
      IVsSolution solution = (IVsSolution)GetService(typeof(SVsSolution));
      var projects = solution.GetProjects();

      foreach (var project in projects)
      {
        var name = project.GetProjectName();
        sb.AppendLine(name);
        var files = project.GetCSharpFiles();

        foreach (var file in files)
        {
          sb.AppendLine($"    {file}");
        }

      }

      var display = new TextDisplay {Text = sb.ToString()};
      display.Show();
    }


    #endregion
  }
}
