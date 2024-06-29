using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using MessagePack;
using Microsoft.VisualStudio.Shell;

namespace Electric.History.ToolWindows.SolutionHistory
{
  [MessagePackObject]
  public class SolutionHistoryItem:INotifyPropertyChanged
  {
    private DateTime mostRecentOpen;
    private int openCount;
    private Icon solutionIcon;

    [Key(0)]
    public string SolutionName { get; set; }

    [Key(1)]
    public DateTime MostRecentOpen
    {
      get => mostRecentOpen;
      set => SetField(ref mostRecentOpen, value);
    }

    [Key(2)]

    public DateTime FirstOpen { get; set; }
    [Key(3)]

    public bool IsFirstOpenImputed { get; set; }

    [Key(4)]
    public int OpenCount
    {
      get => openCount;
      set => SetField(ref openCount, value);
    }

    [Key(5)] 
    public  string  SolutionPath  { get; set; }


    [Key(6)]
    public Icon SolutionIcon
    {
      get => solutionIcon;
      set => SetField(ref solutionIcon, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    public override string ToString()
    {
      return $"{SolutionName} {MostRecentOpen} {OpenCount}";
    }
  }
}
