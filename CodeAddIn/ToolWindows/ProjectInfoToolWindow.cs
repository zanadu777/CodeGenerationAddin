using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using AddIn.Core.VisualStudio;
using CodeAddIn.Gui.ToolWindowControls;
using CodeAddIn.Gui.ToolWindowControls.SelectionInfo;

namespace CodeAddIn.ToolWindows
{
  [Guid("3cf0bdd1-6a91-4e32-808d-0cfb08ded607")]
  public sealed class ProjectInfoToolWindow : ToolWindowPane
  {
    private ElementHost control;

    public ProjectInfoToolWindow() : base(null)
    {
      this.Caption = "My Tool Window";
    }

    public override IWin32Window Window
    {
      get
      {
        if (control == null)
        {
          var wpfControl = new ProjectInfoControl();
          control = new ElementHost { Child = wpfControl, Dock = DockStyle.Fill };
        }

        return control;
      }
    }

  }
}