using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Extensions
{
  public static  class OleMenuCommandServiceExtensions
  {
    public static void AddCommand(this OleMenuCommandService commandService, int commandId, EventHandler invokeHandler)
    {
      if (commandService != null)
      {
        var cmdId = new CommandID(PackageGuids.CmdSet, commandId);
        var menuItem = new MenuCommand(invokeHandler, cmdId);
        commandService.AddCommand(menuItem);
      }
    }
  }
}
