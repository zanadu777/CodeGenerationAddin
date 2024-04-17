using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.VisualStudio
{
  public interface IVsItemDisplay
  {
    VsItem Item {  set; }
  }
}
