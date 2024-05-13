using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.Shell;
using Electric.Navigation.Controls;

namespace Electric.Navigation.ToolWindows
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
  [Guid("423acdf7-e915-46dc-80de-c31c542b9af1")]
  public class SearchToolWindow : ToolWindowPane
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchToolWindow"/> class.
    /// </summary>
    public SearchToolWindow() : base(null)
    {
      this.Caption = "SearchToolWindow";

      // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
      // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
      // the object returned by the Content property.
      this.Content = new SearchToolWindowControl();
    }

    public string SearchText
    {
      get => ((SearchToolWindowControl) Content).SearchText;
      set => ((SearchToolWindowControl) Content).SearchText = value;
    }
  }
}
