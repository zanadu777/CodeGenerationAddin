using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using EnvDTE;

namespace AddIn.Core.Hierarchy
{
  public class Forest<T>
  {

    private Branch<T> root = new Branch<T>();

    public List<GroupBy<T>> GroupBys { get; set; } = new();

    private List<HashSet<string>> branchesHashes = new List<HashSet<string>>();

    public Func<T, string> LeafHeader { get; set; }

    private Dictionary<List<string>, List<T>>
      itemDict = new Dictionary<List<string>, List<T>>(new ListEqualityComparer());

    public void Add(IEnumerable<T> items)
    {
      for (int i = 0; i < GroupBys.Count; i++)
        branchesHashes.Add(new HashSet<string>());

      foreach (var item in items)
      {
        var keyList = new List<string>();
        foreach (var groupBy in GroupBys)
          keyList.Add(groupBy.GroupByMethod(item));

        if (!itemDict.ContainsKey(keyList))
          itemDict[keyList] = new List<T>();
        itemDict[keyList].Add(item);  
      }

      var sortedLists = itemDict.Keys.OrderBy(list =>
      {
        string key = "";
        for (int i = 0; i < list.Count; i++)
        {
          key += list[i];
        }
        return key;
      }).ToList();

      foreach (var sortedList in sortedLists)
      {
        var activBranch = root;
        foreach (var level in sortedList)
          activBranch = activBranch.AddSubBranch(level);

        activBranch.Leaves.AddRange(itemDict[sortedList]);
      }
    }

    public List<System.Windows.Controls.TreeViewItem> ExportToTreeViewItems()
    {
      var items = new List<TreeViewItem>();
      var activeBranch = root;
      foreach (var branch in activeBranch.Branches)
        items.Add(Convert(branch));

      return items; 
    }

    private TreeViewItem Convert(Branch<T> branch)
    {
      var item = new TreeViewItem();
      item.Header = branch.Name;
      item.IsExpanded = true;

      foreach (var subBranch in branch.Branches)
      {
        item.Items.Add(Convert(subBranch));
      }

      foreach (var leaf in branch.Leaves)
      {
        var tvLeaf = new TreeViewItem
        {
          Header = LeafHeader(leaf),
          Tag = leaf
        };
        item.Items.Add(tvLeaf);
      }

      return item;
    }
    public void Add(T item)
    {

    }


    public override string ToString()
    {
      return base.ToString();
    }

  }
}
