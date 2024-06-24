using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace AddIn.Core
{
  public class CodeElementLocation
  {
    public string ElementType { get; set; }

    public string ProjectName { get; set; }
    public string NameSpace { get; set; }

    public string TypeName { get; set; }
    
    public string SolutionRelativePath { get; set; }
  }
}
