using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Hierarchy
{
  public class ListEqualityComparer : IEqualityComparer<List<string>>
  {
    public bool Equals(List<string> x, List<string> y)
    {
      return x.SequenceEqual(y);
    }

    public int GetHashCode(List<string> obj)
    {
      int hash = 19;
      foreach (var val in obj)
      {
        hash = hash * 31 + (val == null ? 0 : val.GetHashCode());
      }
      return hash;
    }
  }
}
