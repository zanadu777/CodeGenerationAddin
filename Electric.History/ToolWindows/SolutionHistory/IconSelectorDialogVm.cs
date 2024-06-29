using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Electric.History.ToolWindows.SolutionHistory
{
  public class IconSelectorDialogVm : INotifyPropertyChanged
  {
    private IconSet selectedIconSet;
    private ObservableCollection<Icon> icons = new();

    public ObservableCollection<Icon> Icons
    {
      get => icons;
      set => SetField(ref icons, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<IconSet> IconSets { get; set; } = new();

    public IconSet SelectedIconSet
    {
      get { return selectedIconSet; }
      set
      {
        selectedIconSet = value;
        Icons = selectedIconSet.Icons;
      }
    }

    public IconSelectorDialogVm()
    {
      var colors = new List<string> { "gold", "orange", "aqua", "lavender", "red" };

      var boltIconSet = new IconSet { Name = "Bolt", DisplayIcon = new Icon { Shape = "bolt", Color1 = "gold" } };
      for (int i = 0; i < colors.Count; i++)
        boltIconSet.Icons.Add(new Icon { Shape = "bolt", Color1 = colors[i] });
      IconSets.Add(boltIconSet);


      var boltsIconSet = new IconSet { Name = "Bolts", DisplayIcon = new Icon { Shape = "bolts", Color1 = "gold", Color2 = "orange" } };
      for (int iRow = 0; iRow < colors.Count; iRow++)
      {
        for (int iCol = 0; iCol < colors.Count; iCol++)
        {
          boltsIconSet.Icons.Add(new Icon { Shape = "bolts", Color1 = colors[iRow], Color2 = colors[iCol] });
        }
      }
      IconSets.Add(boltsIconSet);

      var circleBoltSet = new IconSet { Name = "Circle Bolt", DisplayIcon = new Icon { Shape = "circleBolt", Color1 = "gold", Color2 = "orange" } };
      for (int iRow = 0; iRow < colors.Count; iRow++)
      {
        for (int iCol = 0; iCol < colors.Count; iCol++)
        {
          if (iRow == iCol)
            circleBoltSet.Icons.Add(new Icon { Shape = "none"  });
          else
            circleBoltSet.Icons.Add(new Icon { Shape = "circleBolt", Color1 = colors[iRow], Color2 = colors[iCol] });
        }
      }
      IconSets.Add(circleBoltSet);


      var squareBoltSet = new IconSet { Name = "Square Bolt", DisplayIcon = new Icon { Shape = "squareBolt", Color1 = "gold", Color2 = "orange" } };
      for (int iRow = 0; iRow < colors.Count; iRow++)
      {
        for (int iCol = 0; iCol < colors.Count; iCol++)
        {
          if (iRow == iCol)
            squareBoltSet.Icons.Add(new Icon { Shape = "none" });
          else
            squareBoltSet.Icons.Add(new Icon { Shape = "squareBolt", Color1 = colors[iRow], Color2 = colors[iCol] });
        }
      }
      IconSets.Add(squareBoltSet);

      SelectedIconSet = IconSets[0];
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
