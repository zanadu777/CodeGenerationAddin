using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MessagePack;

namespace Electric.History.ToolWindows.SolutionHistory
{
  [MessagePackObject]
  public class SolutionHistory
  {
    public event EventHandler SolutionUpdated;
    public event EventHandler SolutionCleared;

    [Key(0)]
    public List<SolutionHistoryItem> SerializableSolutions
    {
      get => Solutions.ToList();
      set => Solutions = new ObservableCollection<SolutionHistoryItem>(value ?? new List<SolutionHistoryItem>());
    }


    [IgnoreMember]
    public ObservableCollection<SolutionHistoryItem> Solutions { get; private set; } = new ObservableCollection<SolutionHistoryItem>();

    public void AddOpenEvent(string currentSolutionFullName, DateTime timeOpened)
    {
      var solutionName = Path.GetFileNameWithoutExtension(currentSolutionFullName);

      var current = (from SolutionHistoryItem s in Solutions
        where s.SolutionName == solutionName && s.SolutionPath == currentSolutionFullName
                     select s).FirstOrDefault();

      if (current == null)
      {
        var solution = new SolutionHistoryItem
        {
          SolutionName = solutionName,
          SolutionPath= currentSolutionFullName ,
          FirstOpen = File.GetCreationTimeUtc(currentSolutionFullName),
          IsFirstOpenImputed = true,
          MostRecentOpen = timeOpened,
          OpenCount = 1
        };
        Solutions.Insert(0,solution);
      }
      else
      {
        current.MostRecentOpen = timeOpened;
        current.OpenCount++;
        var currentPos = Solutions.IndexOf(current);
        Solutions.Move(currentPos, 0);
      }

      OnSolutionUpdated(EventArgs.Empty);
    }

    public void ClearAll()
    {
      Solutions.Clear();
      OnSolutionCleared(EventArgs.Empty);
    }

    protected virtual void OnSolutionUpdated(EventArgs e)
    {
      SolutionUpdated?.Invoke(this, e);
    }

    protected virtual void OnSolutionCleared(EventArgs e)
    {
      SolutionCleared?.Invoke(this, e);
    }
  }
}
