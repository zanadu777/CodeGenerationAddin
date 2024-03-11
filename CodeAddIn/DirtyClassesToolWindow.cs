using CodeModel;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CodeAddIn
{
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
