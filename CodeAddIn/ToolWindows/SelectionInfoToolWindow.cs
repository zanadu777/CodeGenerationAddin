using CodeModel;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using CodeAddIn.Gui.ToolWindowControls;
using AddIn.Core.VisualStudio;
using CodeAddIn.Gui.ToolWindowControls.SelectionInfo;

namespace CodeAddIn.ToolWindows
{
  [Guid("9c4709fd-95e7-461b-a86e-6aebd726f802")]
  public sealed class SelectionInfoToolWindow : ToolWindowPane
  {
    private ElementHost host;
    private IVsItemDisplay wpfControl;

    public SelectionInfoToolWindow() : base(null)
    {
      this.Caption = "Current Selection";
      VS.SelectionEvents.OnChange += SelectionChanged;
    }

    public override IWin32Window Window
    {
      get
      {
        if (host == null)
        {
          wpfControl = new SelectionInfoControl();
          host = new ElementHost { Child = (UIElement)wpfControl  , Dock = DockStyle.Fill };
        }

        return host;
      }
    }

    private void SelectionChanged()
    {
      wpfControl.Item = VS.SelectedItem;
    }

    protected override void OnClose()
    {
      VS.SelectionEvents.OnChange -= SelectionChanged;

      base.OnClose();
    }

  }
}