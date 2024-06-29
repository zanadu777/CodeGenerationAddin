using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Electric.History.ToolWindows.SolutionHistory
{
  /// <summary>
  /// Interaction logic for IconSelectorDialog.xaml
  /// </summary>
  public partial class IconSelectorDialog : Window
  {
    public IconSelectorDialog( )
    {
      InitializeComponent();
      DataContext = new IconSelectorDialogVm( );
      
    }

    public Icon SelectedIcon { get; set; }

    public List<Icon> Icons { get; set; } = new();


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Icon selectedIcon = e.AddedItems[0] as Icon;

      if (selectedIcon != null)
      {
        SelectedIcon = selectedIcon;
        this.DialogResult = true;
      }
    }
  }
}
