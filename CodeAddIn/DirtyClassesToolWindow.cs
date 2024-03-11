using CodeModel;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CodeAddIn
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
  [Guid("9352258f-3d02-4e83-a341-b262ad8b84c5")]
  public sealed class DirtyClassesToolWindow : ToolWindowPane
  {
    private ElementHost control;

    public DirtyClassesToolWindow() : base(null)
    {
      this.Caption = "My Tool Window";
    }

 

    public override IWin32Window Window
    {
      get
      {
        if (control == null)
        {
          var wpfControl = new TestUserControl();
          control = new ElementHost { Child = wpfControl, Dock = DockStyle.Fill };
        }

        return control;
      }
    }

  }
}
