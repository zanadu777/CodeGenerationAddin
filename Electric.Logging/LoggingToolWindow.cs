using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace Electric.Logging
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
  [Guid("c0815a37-da47-4e1c-bbc5-0a3471088b06")]
  public class LoggingToolWindow : ToolWindowPane
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingToolWindow"/> class.
    /// </summary>
    public LoggingToolWindow() : base(null)
    {
      this.Caption = "LoggingToolWindow";

      // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
      // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
      // the object returned by the Content property.
      this.Content = new LoggingToolWindowControl();
    }
  }
}
