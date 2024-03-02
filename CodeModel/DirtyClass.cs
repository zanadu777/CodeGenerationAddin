using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeModel
{
  public class DirtyClass
  {
    public string Name { get; set; }
    public string FullName { get; set; }
    public AssemblyName AssemblyName { get; set; }

    public DateTime LastModified { get; set; }

  }
}
