using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core.Records;
using EnvDTE;

namespace Electric.Navigation.ToolWindows
{
  public class SearchType
  {
    public string Name { get; set; }

    public Func<IEnumerable<ProjectItem>, string , IEnumerable<SearchResult>> GetSearchResults { get; set; }
  }
}
