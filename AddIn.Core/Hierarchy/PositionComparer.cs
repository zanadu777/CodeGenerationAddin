using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Hierarchy
{
  public class PositionComparer : IComparer<List<string>>
  {
    private readonly List<int> positions;

    public PositionComparer(List<int> positions)
    {
      this.positions = positions;
    }

    public int Compare(List<string> x, List<string> y)
    {
      foreach (var position in positions)
      {
        int result = x[position].CompareTo(y[position]);
        if (result != 0)
        {
          return result;
        }
      }
      return 0;
    }
  }
}
