using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AddIn.Core.Extensions;
using AddIn.Core.Helpers;
using AddIn.Core.Hierarchy;
using AddIn.Core.Records;
using EnvDTE;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Microsoft.VisualStudio.Shell;

namespace Electric.Navigation.ToolWindows
{
  /// <summary>
  /// Interaction logic for SearchToolWindowControl.
  /// </summary>
  public partial class SearchToolWindowControl : UserControl
  {
    private DTE dte;

    #region Properties
    public ObservableCollection<SearchType> SearchTypes { get; set; } = new ObservableCollection<SearchType>();
    public ObservableCollection<SearchLocation> SearchLocations { get; set; } = new ObservableCollection<SearchLocation>();

    private string currentSolutionName;
    public static readonly DependencyProperty LastNSearchesProperty = DependencyProperty.Register(
      nameof(LastNSearches), typeof(LastN<string>), typeof(SearchToolWindowControl), new PropertyMetadata(new LastN<string>(12)));

    public LastN<string> LastNSearches
    {
      get { return (LastN<string>)GetValue(LastNSearchesProperty); }
      set { SetValue(LastNSearchesProperty, value); }
    }

    public static readonly DependencyProperty isPreviousSearchAvailableProperty = DependencyProperty.Register(
      nameof(IsPreviousSearchAvailable), typeof(bool), typeof(SearchToolWindowControl), new PropertyMetadata(default(bool)));

    public bool IsPreviousSearchAvailable
    {
      get { return (bool)GetValue(isPreviousSearchAvailableProperty); }
      set { SetValue(isPreviousSearchAvailableProperty, value); }
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
      get => (string)GetValue(StatusMessageProperty);
      set => SetValue(StatusMessageProperty, value);
    }

    public static readonly DependencyProperty SearchResultsItemsProperty = DependencyProperty.Register(
      nameof(SearchResultsItems), typeof(List<TreeViewItem>), typeof(SearchToolWindowControl), new PropertyMetadata(default(List<TreeViewItem>)));

    public List<TreeViewItem> SearchResultsItems
    {
      get { return (List<TreeViewItem>) GetValue(SearchResultsItemsProperty); }
      set { SetValue(SearchResultsItemsProperty, value); }
    }
    #endregion

    static SearchToolWindowControl()
    {
      var options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
        new IMessagePackFormatter[] { new LastNFormatter<string>() }, // Add your formatter here.
        new[] { StandardResolver.Instance }
      ));

      MessagePackSerializer.DefaultOptions = options;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SearchToolWindowControl"/> class.
    /// </summary>
    public SearchToolWindowControl()
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      this.InitializeComponent();
      dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
      currentSolutionName = Path.GetFileNameWithoutExtension(dte.Solution.FullName);
      //SearchText
      if (!string.IsNullOrWhiteSpace(currentSolutionName))
        InitializeLastNSearchesAsync(currentSolutionName);

      //SearchTypes
      SearchTypes.Add(new SearchType { Name = "Case Insensitive", GetSearchResults = (p, s, d) => p.GetSearchResultsCaseInsensitiveParallel(s, d) });
      SearchTypes.Add(new SearchType { Name = "Case Sensitive", GetSearchResults = (p, s, d) => p.GetSearchResultsCaseSensitive(s, d) });
      SearchTypes.Add(new SearchType { Name = "Regex", GetSearchResults = (p, s, d) => p.GetSearchResultsRegexParallel(s, d) });
      SearchTypes.Add(new SearchType { Name = "Whole Word (Case Insensitive)", GetSearchResults = (p, s,d) => p.GetSearchResultsWholeWord(s,d, false) });
      SearchTypes.Add(new SearchType { Name = "Whole Word (Case Sensitive)", GetSearchResults = (p, s, d) => p.GetSearchResultsWholeWord(s, d, true) });
      SelectedSearchType = SearchTypes[0];

      //SearchLocation
      InitializeSearchLocation();

      var solutionEvents = dte.Events.SolutionEvents;
      solutionEvents.Opened += SolutionOpened;
    }

    private void SolutionOpened()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      currentSolutionName = Path.GetFileNameWithoutExtension(dte.Solution.FullName);
      InitializeLastNSearchesAsync(currentSolutionName);
      tvSearchResults.Items.Clear();

    }

    public void InitializeSearchLocation()
    {

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

      SearchLocations.Add(new SearchLocation
      {
        Name = "Active Document",
        GetProjectItems = () =>
      {
        ThreadHelper.ThrowIfNotOnUIThread();
        return new List<ProjectItem> { dte.ActiveDocument.ProjectItem };
      }
      });

      SearchLocations.Add(new SearchLocation
      {
        Name = "Open Documents",
        GetProjectItems = () =>
        {
          ThreadHelper.ThrowIfNotOnUIThread();
          return new List<ProjectItem>(dte.GetOpenProjectItems());
        }
      });

      SelectedSearchLocation = SearchLocations[0];
    }

#pragma warning disable VSTHRD100
#pragma warning disable VSTHRD200
    private async void InitializeLastNSearchesAsync(string solutionName)
#pragma warning restore VSTHRD200
#pragma warning restore VSTHRD100
    {
      var lastNSearches = await IsolatedStorageHelper.DeserializeFromIsolatedStorageAsync<LastN<string>>($"Electric.Navigation.LastNSearches.{solutionName}");
      if (lastNSearches != null)
        LastNSearches = lastNSearches;
      else
        LastNSearches = new LastN<string>(12);

      IsPreviousSearchAvailable = LastNSearches.Items.Any();
    }

    private async void Search(object sender, RoutedEventArgs e)
    {
      if (String.IsNullOrWhiteSpace(SearchText))
        return;

      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var swatch = Stopwatch.StartNew();
      var projectItems = SelectedSearchLocation.GetProjectItems().Where(p => p.FileCount > 0).ToList();

      var filenames = projectItems.DistinctFileNames().ToList();
      var contentDictP = GetNameDictFileContentsParallel(filenames);

      SearchResults = SelectedSearchType.GetSearchResults(projectItems, SearchText, contentDictP).ToList();

      var forest = new Forest<SearchResult>();
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod = x => x.Project });
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod = x => x.File });

      forest.LeafHeader = x => x.Code;

      forest.Add(SearchResults);
      SearchResultsItems = forest.ExportToTreeViewItems();
      tvSearchResults.Items.Clear();
      tvSearchResults.ItemsSource = SearchResultsItems;

      LastNSearches.Add(SearchText);
      StatusMessage = $"Search Completed and found {SearchResults.Count()} matches in {swatch.Elapsed.TotalSeconds:F2}";

      var name = Path.GetFileNameWithoutExtension(dte.Solution.FullName);
      await IsolatedStorageHelper.SerializeToIsolatedStorageAsync(LastNSearches, $"Electric.Navigation.LastNSearches.{name}");
      IsPreviousSearchAvailable = true;
    }

    private bool IsFileEmpty(string fileName)
    {
      using (var fileStream = File.OpenRead(fileName))
      {
        return fileStream.ReadByte() == -1;
      }
    }

    private ConcurrentDictionary<string, string> GetNameDictFileContentsParallel(IEnumerable<string> fileNames)
    {
      var fileContentsDict = new ConcurrentDictionary<string, string>();

      Parallel.ForEach(fileNames, fileName =>
      {
        if (!File.Exists(fileName))
          return;

        string fileText = File.ReadAllText(fileName);
        fileContentsDict[fileName] = fileText;
      });

      return fileContentsDict;
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
      bool? result = dlg.ShowDialog();

      if (result == true)
      {
        string filename = dlg.FileName;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(SearchResults);
        File.WriteAllText(filename, json);
      }
    }

    private void tvSearchResults_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      var treeViewItem = e.NewValue as TreeViewItem;
      if (treeViewItem == null)
        return;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine("   " + NodeText(treeViewItem));
    }

    private void GoToAssociated(TreeViewItem treeViewItem)
    {
      var searchResult = treeViewItem.Tag as SearchResult;
      if (searchResult == null)
        return;

      dte.GoTo(searchResult);
    }

    private void SearchToolWindowControl_OnLoaded(object sender, RoutedEventArgs e)
    {

    }

    private void tvSearchResults_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var treeview = sender as TreeView;
      if (treeview == null)
        return;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine(NodeText(treeview.SelectedItem as TreeViewItem));
    }

    private void tvSearchResults_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var treeview = sender as TreeView;
      if (treeview == null)
        return;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine($"   {NodeText(treeview.SelectedItem as TreeViewItem)}");
    }

    private void tvSearchResults_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var treeview = sender as TreeView;
      if (treeview == null)
        return;

      //for reasons unknown the selection changed event does not always occur. This corrects that.
      Point mousePosition = e.GetPosition(treeview);
      HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeview, mousePosition);
      var treeViewItemFromHit = hitTestResult.VisualHit.GetParentOfType<TreeViewItem>();

      var treeViewItem = treeview.SelectedItem as TreeViewItem;
      if (treeViewItem != treeViewItemFromHit)
        treeViewItemFromHit.IsSelected = true;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine($"   {NodeText(treeViewItemFromHit)}");

      if (treeViewItemFromHit is null)
        return;

      GoToAssociated(treeViewItemFromHit);
    }

    private void tvSearchResults_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var treeview = sender as TreeView;
      if (treeview == null)
        return;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine($"   {NodeText(treeview.SelectedItem as TreeViewItem)}");
    }

    private void tvSearchResults_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var treeview = sender as TreeView;
      if (treeview == null)
        return;

      Debug.WriteLine(MethodBase.GetCurrentMethod()?.Name);
      Debug.WriteLine($"   {NodeText(treeview.SelectedItem as TreeViewItem)}");
    }

    private string NodeText(TreeViewItem item)
    {
      if (item == null)
        return string.Empty;

      if (item.Tag == null)
        return item.Header.ToString();

      else
        return ((SearchResult)item.Tag).ToString();
    }


    private void cmbPrevious_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

      if (e.AddedItems.Count <= 0)
        return;

      var selected = e.AddedItems[0] as string;
      if (!string.IsNullOrWhiteSpace(selected))
        this.SearchText = selected;
    }
  }
}