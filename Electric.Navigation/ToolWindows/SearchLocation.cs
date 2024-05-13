using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace Electric.Navigation.ToolWindows
{
  public  class SearchLocation
  {
    public string Name { get; set; }

    public Func<IEnumerable<ProjectItem>> GetProjectItems { get; set; }
  }
}
