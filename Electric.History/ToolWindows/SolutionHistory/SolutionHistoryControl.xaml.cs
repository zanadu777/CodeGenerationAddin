using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using AddIn.Core.Helpers;

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
      HistoryPackage.SolutionHistory.Solutions.Clear();
      HistoryPackage.SolutionHistory.UpdateSolution();
    }

    private void SetIcon(object sender, RoutedEventArgs e)
    {
      var selectedItems = SelectedItems(sender, e);
      var dialog = new IconSelectorDialog();

      var dialogResult = dialog.ShowDialog();
      if (dialogResult == true)
        foreach (var item in selectedItems)
          item.SolutionIcon = dialog.SelectedIcon;
      else
        foreach (var item in selectedItems)
          item.SolutionIcon = null;

      HistoryPackage.SolutionHistory.UpdateSolution();
    }


    private List<SolutionHistoryItem> SelectedItems(object sender, RoutedEventArgs e)
    {
      if (sender is MenuItem menuItem)
      {
        var contextMenu = menuItem.Parent as ContextMenu;
        if (contextMenu?.PlacementTarget is DataGrid grid)
        {
          var selectedItems = grid.SelectedItems.Cast<SolutionHistoryItem>().ToList();
          return selectedItems;
        }

      }

      return new List<SolutionHistoryItem>();
    }

    private void SaveAsClick(object sender, RoutedEventArgs e)
    {
      WpfFileSource.SerializeToFile(HistoryPackage.SolutionHistory);
    }

    private void LoadFromClick(object sender, RoutedEventArgs e)
    {
      var updatedHistory = WpfFileSource.DeserializeFile<SolutionHistory>();
      HistoryPackage.SolutionHistory.Solutions.Clear();
      foreach (var item in updatedHistory.Solutions)
        HistoryPackage.SolutionHistory.Solutions.Add(item);

      HistoryPackage.SolutionHistory.UpdateSolution();
    }

    private void Remove(object sender, RoutedEventArgs e)
    {
      var selectedItems = SelectedItems(sender, e);

      foreach (var solution in selectedItems)
        HistoryPackage.SolutionHistory.Solutions.Remove(solution);

      HistoryPackage.SolutionHistory.UpdateSolution();
    }
  }
}
