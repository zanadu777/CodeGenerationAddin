using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace Electric.History.ToolWindows.SolutionHistory
{
  /// <summary>
  /// Interaction logic for SolutionHistoryControl.xaml
  /// </summary>
  public partial class SolutionHistoryControl : UserControl
  {
    private string currentSolutionName;

    private readonly IServiceProvider serviceProvider;
    public SolutionHistoryControl(IServiceProvider serviceProvider)
    {
      InitializeComponent();
      this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      HistoryGrid.ItemsSource = HistoryPackage.SolutionHistory.Solutions;
      HistoryPackage.SolutionHistory.SolutionUpdated += SolutionHistory_SolutionUpdated;
      //var solutionEvents = dte.Events.SolutionEvents;
      //solutionEvents.Opened += SolutionOpened;
    }

    private void SolutionHistory_SolutionUpdated(object sender, System.EventArgs e)
    {
      HistoryGrid.ItemsSource = HistoryPackage.SolutionHistory.Solutions;
    }



    private void ToggleTimeOpenedVisibility(object sender, RoutedEventArgs e)
    {
      var timeOpenedColumn = HistoryGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Time Opened");
      if (timeOpenedColumn != null)
      {
        timeOpenedColumn.Visibility = timeOpenedColumn.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    private async void HistoryGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var source = e.OriginalSource as DependencyObject;
      while (source != null && !(source is DataGridRow))
      {
        source = VisualTreeHelper.GetParent(source);
      }

      if (source is DataGridRow row)
      {
        var item = (SolutionHistoryItem)row.Item;
        DTE2 dte = await GetDTEAsync();

        if (dte != null)
        {
          dte.Solution.Open(item.SolutionPath);
        }
      }
    }

    private async Task<DTE2> GetDTEAsync()
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      DTE dte = (DTE)await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(DTE));
      return (DTE2)dte;
    }

    private void HistoryGrid_Loaded(object sender, RoutedEventArgs e)
    {
      if (HistoryGrid.Columns.Count > 0)
        HistoryGrid.Columns[HistoryGrid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
    }

    private void ClearAll(object sender, RoutedEventArgs e)
    {

    }

    private void SetIcon(object sender, RoutedEventArgs e)
    {
      if (sender is MenuItem menuItem)
      {
        var contextMenu = menuItem.Parent as ContextMenu;
        if (contextMenu?.PlacementTarget is DataGrid grid)
        {
          var selectedItems = grid.SelectedItems.Cast<SolutionHistoryItem>().ToList(); ;

          var dialog = new IconSelectorDialog();
          dialog.ShowDialog();

        }
      }
    }
  }
}
