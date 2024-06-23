using CodeModel;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using AddIn.Core.VisualStudio;
using CodeAddIn.Gui.ToolWindowControls;
using EnvDTE;



namespace CodeAddIn.ToolWindows
{
  [Guid("ec9971ad-3c7a-4f9a-bae0-76ee30ccb30a")]
  public sealed class SolutionInfoToolWindow : ToolWindowPane
  {
    private ElementHost control;
    private SolutionInfoControl solutionInfoControl;

    public SolutionInfoToolWindow() : base(null)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      this.Caption = "My Tool Window";
      VS.SelectionEvents.OnChange += SelectionChanged;
    }

    public override IWin32Window Window
    {
      get
      {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (control == null)
        {
          solutionInfoControl = new SolutionInfoControl();
          control = new ElementHost { Child = solutionInfoControl, Dock = DockStyle.Fill };
        }

        var selectedItem = VS.GetSelectedProjectItem();
        solutionInfoControl.SelectedItem = selectedItem?.Name;
        solutionInfoControl.VsItem = VS.SelectedItem;
        return control;
      }
    }



    private void SelectionChanged()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var selectedItem =VS.SelectedItem;
      solutionInfoControl.SelectedItem = $"{selectedItem?.Name} {selectedItem?.Type}";
      solutionInfoControl.VsItem = VS.SelectedItem;
    }

    protected override void OnClose()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      // Unsubscribe from the event when the tool window is closed
      VS.SelectionEvents.OnChange -= SelectionChanged;

      base.OnClose();
    }

  }
}