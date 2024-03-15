using AddIn.Core.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeAddIn.Gui.ToolWindowControls
{
  /// <summary>
  /// Interaction logic for SolutionInfoControl.xaml
  /// </summary>
  public partial class SolutionInfoControl : UserControl
  {
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      "SelectedItem", typeof(string), typeof(SolutionInfoControl), new PropertyMetadata(default(string)));

    public string SelectedItem
    {
      get { return (string)GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty VsItemProperty = DependencyProperty.Register(
      nameof(VsItem), typeof(VsItem), typeof(SolutionInfoControl), new PropertyMetadata(default(VsItem)));

    public VsItem VsItem
    {
      get { return (VsItem) GetValue(VsItemProperty); }
      set { SetValue(VsItemProperty, value); }
    }
    public SolutionInfoControl()
    {
      InitializeComponent();
    }
  }
}
