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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ExtensionManager;
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
      ToggleColumnVisibility("Time Opened");
    }

    private void ToggleColumnVisibility(string columnHeader)
    {
      var column = HistoryGrid.Columns.FirstOrDefault(c => c.Header.ToString() == columnHeader);
      if (column != null)
      {
        column.Visibility = column.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
      }
    }


    private void ToggleFullPathVisibility(object sender, RoutedEventArgs e)
    {
      ToggleColumnVisibility("Full Path");
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

    private async void OpenInNewWindow(object sender, RoutedEventArgs e)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var selectedItems = SelectedItems(sender, e);

      if (selectedItems.Count > 0)
      {
        var item = selectedItems.First();
        var solutionPath = item.SolutionPath;

        if (!string.IsNullOrEmpty(solutionPath))
        {
          try
          {
            var vsPath = await GetVisualStudioPathAsync();

            if (!string.IsNullOrEmpty(vsPath))
              System.Diagnostics.Process.Start(vsPath, $"\"{solutionPath}\"");
            else
              MessageBox.Show("Visual Studio executable not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Failed to open solution in a new instance of Visual Studio. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
    }

    private async Task<string> GetVisualStudioPathAsync()
    {

      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
      string vsPath = currentProcess.MainModule.FileName;
      return vsPath;
    }

    private async void ShowVersion(object sender, RoutedEventArgs e)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

      try
      {
        IVsExtensionManager extensionManager = await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(SVsExtensionManager)) as IVsExtensionManager;
        if (extensionManager != null)
        {
          var installedExtensions = extensionManager.GetInstalledExtensions();
          var extension = installedExtensions.FirstOrDefault(ext => ext.Header.Identifier == "Electric.History.7531b605-879e-4a53-bfb7-842f7cad1ba4"); 
          if (extension != null)
          {
            string version = extension.Header.Version.ToString();
            MessageBox.Show($"Version: {version}", "Version", MessageBoxButton.OK, MessageBoxImage.Information);
          }
          else
          {
            MessageBox.Show("Extension not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
        else
        {
          MessageBox.Show("Failed to get the extension manager service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to get VSIX version. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
