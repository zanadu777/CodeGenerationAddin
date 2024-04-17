using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAddIn.Gui.ToolWindowControls.SelectionInfo
{
  public class NodeViewModel
  {
    public string Name { get; set; }
    public bool IsExpanded { get; set; } = true;
    public ObservableCollection<NodeViewModel> Children { get; set; } = new ObservableCollection<NodeViewModel>();
  }
}
