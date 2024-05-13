using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Hierarchy
{
  public class GroupBy<T>
  {
    public Func<T, string> GroupByMethod { get; set; }
  }
}
