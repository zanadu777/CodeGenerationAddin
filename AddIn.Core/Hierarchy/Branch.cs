using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Hierarchy
{
  public  class Branch<T>
  {
    public string Name { get; set; }

    private Dictionary<string, Branch<T>> branchDict = new();
    public Branch<T> AddSubBranch(string key)
    {
      if (branchDict.ContainsKey(key))
        return branchDict[key];

      var branch = new Branch<T> {Name = key};
      Branches.Add(branch);
      branchDict[key] = branch;
      return branch;
    }
    public List<Branch<T>> Branches { get; set; } = new List<Branch<T>>();
    public List< T> Leaves { get; set; } = new ();
  }
}
