using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AddIn.Core.Extensions;
using AddIn.Core.Hierarchy;
using AddIn.Core.Records;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Electric.Navigation.ToolWindows
{
  /// <summary>
  /// Interaction logic for SearchToolWindowControl.
  /// </summary>
  public partial class SearchToolWindowControl : UserControl
  {
    private DTE dte;
    public ObservableCollection<SearchType> SearchTypes { get; set; } = new ObservableCollection<SearchType>();
    public ObservableCollection<SearchLocation> SearchLocations { get; set; } = new ObservableCollection<SearchLocation>();
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchToolWindowControl"/> class.
    /// </summary>
    public SearchToolWindowControl()
    {
      this.InitializeComponent();
      dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
      SearchTypes.Add(new SearchType { Name = "Case Insensitive", GetSearchResults = (p, s) => p.GetSearchResultsCaseInsensitive(s) });
      SearchTypes.Add(new SearchType
      {
        Name = "Case Sensitive",
        GetSearchResults = (p, s) =>
        {
          var filteredItems = p.WhereText(t => t.Contains(s));
          return filteredItems.GetSearchResultsCaseSensitive(s);
        }
      });

      SearchTypes.Add(new SearchType { Name = "Regex", GetSearchResults = (p, s) => p.GetSearchResultsRegex(s) });
      SearchTypes.Add(new SearchType { Name = "Whole Word (Case Insensitive)", GetSearchResults = (p, s) => p.GetSearchResultsWholeWord(s, false) });
      SearchTypes.Add(new SearchType { Name = "Whole Word (Case Sensitive)", GetSearchResults = (p, s) => p.GetSearchResultsWholeWord(s, true) });
      SelectedSearchType = SearchTypes[0];


      SearchLocations.Add(new SearchLocation
      {
        Name = "Entire Solution",
        GetProjectItems = () =>
        {
          ThreadHelper.ThrowIfNotOnUIThread();
          var solution = dte.IVsSolution();
          var projectItems = solution.GetAllProjectItems();
          return projectItems;
        }

      });
      SearchLocations.Add(new SearchLocation { Name = "Active Document", GetProjectItems = () =>
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        return new List<ProjectItem> { dte.ActiveDocument.ProjectItem };
      }
      });
      SelectedSearchLocation = SearchLocations[0];
    }

    public static readonly DependencyProperty selectedSearchTypeProperty = DependencyProperty.Register(
      nameof(SelectedSearchType), typeof(SearchType), typeof(SearchToolWindowControl), new PropertyMetadata(default(SearchType)));

    public SearchType SelectedSearchType
    {
      get { return (SearchType)GetValue(selectedSearchTypeProperty); }
      set { SetValue(selectedSearchTypeProperty, value); }
    }

    public static readonly DependencyProperty selectedSearchLocationProperty = DependencyProperty.Register(
      nameof(SelectedSearchLocation), typeof(SearchLocation), typeof(SearchToolWindowControl), new PropertyMetadata(default(SearchLocation)));

    public SearchLocation SelectedSearchLocation
    {
      get { return (SearchLocation)GetValue(selectedSearchLocationProperty); }
      set { SetValue(selectedSearchLocationProperty, value); }
    }


    public IEnumerable<SearchResult> SearchResults { get; set; }

    public static readonly DependencyProperty searchTextProperty = DependencyProperty.Register(
      nameof(SearchText), typeof(string), typeof(SearchToolWindowControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty StatusMessageProperty = DependencyProperty.Register(nameof(StatusMessage), typeof(string), typeof(SearchToolWindowControl), new PropertyMetadata(default(string)));

    public string SearchText
    {
      get { return (string)GetValue(searchTextProperty); }
      set { SetValue(searchTextProperty, value); }
    }

    public string StatusMessage
    {
      get => (string) GetValue(StatusMessageProperty);
      set => SetValue(StatusMessageProperty, value);
    }

    private async void Search(object sender, RoutedEventArgs e)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var swatch = Stopwatch.StartNew();
      var projectItems = SelectedSearchLocation.GetProjectItems();

      SearchResults = SelectedSearchType.GetSearchResults(projectItems, SearchText).ToList();


      var forest = new Forest<SearchResult>();
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod = x => x.Project });
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod = x => x.File });

      forest.LeafHeader = x => x.Code;

      forest.Add(SearchResults);
      var items = forest.ExportToTreeViewItems();
      tvSearchResults.Items.Clear();
      foreach (var item in items)
        tvSearchResults.Items.Add(item);

      StatusMessage = $"Search Completed and found {SearchResults.Count()} matches in {swatch.Elapsed.TotalSeconds:F2}";
    }

    private void resultsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var result = e.AddedItems[0] as SearchResult;
      dte.GoTo(result);
    }

    private void mnuSaveAsJson(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
      dlg.FileName = "Document"; // Default file name
      dlg.DefaultExt = ".json"; // Default file extension
      dlg.Filter = "JSON documents (.json)|*.json"; // Filter files by extension
      Nullable<bool> result = dlg.ShowDialog();

      if (result == true)
      {
        string filename = dlg.FileName;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(SearchResults);
        System.IO.File.WriteAllText(filename, json);
      }
    }

    private void tvSearchResults_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      var treeViewItem  = e.NewValue as TreeViewItem;
      if (treeViewItem == null)
        return;

      var searchResult = treeViewItem.Tag as SearchResult;
      if (searchResult == null)
        return;

      dte.GoTo(searchResult); 
    }

    private void SearchToolWindowControl_OnLoaded(object sender, RoutedEventArgs e)
    {
 
    }
  }
}