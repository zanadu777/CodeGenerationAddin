using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AddIn.Core.Helpers;
using Electric.History.ToolWindows;
using Electric.History.ToolWindows.SolutionHistory;
using Microsoft.VisualStudio;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell.Interop;
using MessagePack;

namespace Electric.History
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
  [Guid(HistoryPackage.PackageGuidString)]
  [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
  [ProvideMenuResource("Menus.ctmenu", 1)]
  [ProvideToolWindow(typeof(SolutionHistoryToolWindow))]
  public sealed class HistoryPackage : AsyncPackage, IVsSolutionEvents
  {
    /// <summary>
    /// Electric.HistoryPackage GUID string.
    /// </summary>
    public const string PackageGuidString = "d554680b-4922-4b6d-835b-cd8a32a5575b";
    private uint _solutionEventsCookie;

    public static SolutionHistory SolutionHistory { get; set; } = new SolutionHistory();
    private const string saveLocation = "SolutionHistory.msgpack";

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
      await SolutionHistoryToolWindowCommand.InitializeAsync(this);

      IVsSolution solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
      if (solution != null)
        solution.AdviseSolutionEvents(this, out _solutionEventsCookie);

      SolutionHistory = await IsolatedStorageHelper.DeserializeFromIsolatedStorageAsync<SolutionHistory>(saveLocation) ?? new SolutionHistory();
      SolutionHistory.SolutionCleared += SolutionHistory_SolutionCleared;
                       
    }

    private void SolutionHistory_SolutionCleared(object sender, EventArgs e)
    {
      JoinableTaskFactory.Run(async () =>
      {
        await IsolatedStorageHelper.SerializeToIsolatedStorageAsync(SolutionHistory, saveLocation);
      });
    }

    #region IVsSolution members

    public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
    {
      return VSConstants.S_OK;
    }

    public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
    {
      return VSConstants.S_OK;
    }

    public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
    {
      return VSConstants.S_OK;
    }

    public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
    {
      return VSConstants.S_OK;
    }

    public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
    {
      return VSConstants.S_OK;
    }

    public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
    {
      return VSConstants.S_OK;
    }

    public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      IVsSolution solution = (IVsSolution)GetService(typeof(SVsSolution));
      if (solution != null)
      {
        solution.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile);

        JoinableTaskFactory.Run(async () =>
        {
          await UpdateSolutionHistory(solutionFile, DateTime.Now);
        });

      }

      return VSConstants.S_OK;
    }

    public async Task<SolutionHistory> UpdateSolutionHistory(string fileName, DateTime fileOpenTime)
    {
      var storedHistory = await IsolatedStorageHelper.DeserializeFromIsolatedStorageAsync<SolutionHistory>(
        saveLocation);
      if (storedHistory == null)
        storedHistory = new SolutionHistory();

      storedHistory.AddOpenEvent(fileName, fileOpenTime);
      SolutionHistory.AddOpenEvent(fileName, fileOpenTime);


      await IsolatedStorageHelper.SerializeToIsolatedStorageAsync(storedHistory, saveLocation);

      return storedHistory;
    }

    public int OnBeforeCloseSolution(object pUnkReserved)
    {
      return VSConstants.S_OK;
    }

    public int OnAfterCloseSolution(object pUnkReserved)
    {
      return VSConstants.S_OK;
    }

    public int OnAfterLoadProject(object pStubHierarchy, object pRealHierarchy)
    {
      return VSConstants.S_OK;
    }

    public int OnAfterOpenProject(object pHierarchy, int fAdded)
    {
      return VSConstants.S_OK;
    }

    public int OnBeforeCloseProject(object pHierarchy, int fRemoved)
    {
      return VSConstants.S_OK;
    }

    public int OnBeforeUnloadProject(object pRealHierarchy, object pStubHierarchy)
    {
      return VSConstants.S_OK;
    }

    public int OnQueryCloseProject(object pHierarchy, int fRemoving, ref int pfCancel)
    {
      return VSConstants.S_OK;
    }

    public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
    {
      return VSConstants.S_OK;
    }

    public int OnQueryUnloadProject(object pRealHierarchy, ref int pfCancel)
    {
      return VSConstants.S_OK;
    }
    #endregion
    #endregion
  }
}
