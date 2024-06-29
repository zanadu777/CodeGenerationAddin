using System.Collections.ObjectModel;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace Electric.History.ToolWindows.SolutionHistory
{
  public class IconSet
  {
    public string Name { get; set; }
    public ObservableCollection<Icon> Icons { get; set; } = new();

    public Icon DisplayIcon { get; set; }
  }
}
