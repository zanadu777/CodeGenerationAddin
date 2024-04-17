using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAddIn.Gui.InfoWindows;
using CodeAddIn.Gui.ToolWindowControls.SelectionInfo;

namespace CodeAddIn.Gui.DesignTime
{
  public static class DesignTimeData
  {
    public static ProjectInfoVm  ProjectInfoVm
    {
      get
      {
        return new ProjectInfoVm
        {
          ProjectName = "CodeAddIn"
        };
      }
    }

    public static CSharpInfoVm ClassInfoVm
    {
      get
      {
        return new CSharpInfoVm
        {
          Name = "NiceClass"
        };
      }
    }

    public static XamlInfoVm XamlInfoVm
    {
      get
      {
        return new XamlInfoVm
        {
          Name = "NiceXaml"
        };
      }
    } 
  }
}
