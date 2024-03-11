using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeModel;

namespace CodeAddIn
{
  public class SolutionEventsListener
  {
    public event Action<List<DirtyClass>> SolutionModified;

    // Call this method whenever the solution is modified
    public void OnSolutionModified(List<DirtyClass> dirtyClasses)
    {
      SolutionModified?.Invoke(dirtyClasses);
    }
  }
}
