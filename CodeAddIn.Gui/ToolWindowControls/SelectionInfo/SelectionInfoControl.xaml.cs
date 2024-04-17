using System.Windows;
using System.Windows.Controls;
using AddIn.Core.VisualStudio;

namespace CodeAddIn.Gui.ToolWindowControls.SelectionInfo
{
  /// <summary>
  /// Interaction logic for SelectionInfoControl.xaml
  /// </summary>
  public partial class SelectionInfoControl : UserControl, IVsItemDisplay
  {
    public SelectionInfoControl()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty itemProperty = DependencyProperty.Register(
      nameof(Item), typeof(VsItem), typeof(SelectionInfoControl), new PropertyMetadata(default(VsItem), OnItemChanged));

    public VsItem Item
    {
      get { return (VsItem)GetValue(itemProperty); }
      set { SetValue(itemProperty, value); }
    }

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (SelectionInfoControl)d;
      control.DisplayInfoControl();
    }

    private void DisplayInfoControl()
    {
      if (Item == null)
        return;

      switch (Item.Type)
      {
        case VsItemType.CSharpFile:
        case VsItemType.CodeBehindCSharp:
         InfoContent.Content = new CSharpInfoControl( );
         InfoContent.DataContext = new CSharpInfoVm(Item );
          break;
        case VsItemType.XamlFile:
          InfoContent.Content = new XamlInfoControl( );
          InfoContent.DataContext = new XamlInfoVm(Item);
          break;
      }
    }
  }
}
