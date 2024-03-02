using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAddIn.Gui.InfoWindows;

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

    public static CsharpInfoVm ClassInfoVm
    {
      get
      {
        return new CsharpInfoVm
        {
         ClassName = "NiceClass"
        };
      }
    }
  }
}
