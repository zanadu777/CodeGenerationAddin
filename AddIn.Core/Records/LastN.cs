using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AddIn.Core.Records
{
  public class LastN<T>
  {
    public int Capacity { get; set; }
    private readonly LinkedList<T> linkedList;
    private readonly HashSet<T> hashSet;

    public ObservableCollection<T> Items { get; }

    public LastN(int capacity) : this(capacity, EqualityComparer<T>.Default) { }
    public LastN(int capacity, IEqualityComparer<T> comparer)
    {
      this.Capacity = capacity;
      this.linkedList = new LinkedList<T>();
      this.hashSet = new HashSet<T>(comparer);
      this.Items = new ObservableCollection<T>();
    }

    public void Add(T item)
    {
      if (hashSet.Contains(item))
      {
        linkedList.Remove(item);
        Items.Remove(item);
      }
      else if (linkedList.Count == Capacity)
      {
        T removed = linkedList.First.Value;
        linkedList.RemoveFirst();
        hashSet.Remove(removed);
        Items.Remove(removed);
      }

      linkedList.AddLast(item);
      hashSet.Add(item);

      // Insert the item at the top.
      Items.Insert(0, item);
    }
  }

}