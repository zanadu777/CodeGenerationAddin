using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core.VisualStudio;

namespace CodeAddIn.Gui.ToolWindowControls.SelectionInfo
{
  public class CSharpInfoVm:INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public CSharpInfoVm()
    {
      
    }

    private VsItem vsItem;
    private string name;

    public CSharpInfoVm(VsItem item)
    {
      vsItem = item;
      Name = item.Name;
    }

    public string Name
    {
      get => name;
      set => SetField(ref name, value);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }
  }
}
