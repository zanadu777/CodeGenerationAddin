using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System;

namespace Electric.History.ToolWindows.SolutionHistory
{
  /// <summary>
  /// This class implements the tool window exposed by this package and hosts a user control.
  /// </summary>
  /// <remarks>
  /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
  /// usually implemented by the package implementer.
  /// <para>
  /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
  /// implementation of the IVsUIElementPane interface.
  /// </para>
  /// </remarks>
  [Guid("bd0de41a-57d3-41b5-85b2-f49f78770126")]
  public class SolutionHistoryToolWindow : ToolWindowPane
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionHistoryToolWindow"/> class.
    /// </summary>
    public SolutionHistoryToolWindow( ) : base(null)
    {
      this.Caption = "Solution History";

      // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
      // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
      // the object returned by the Content property.
      this.Content = new SolutionHistoryToolWindowControl(this);
    }
  }
}
