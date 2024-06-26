using MessagePack;

namespace Electric.History.ToolWindows.SolutionHistory
{
  [MessagePackObject]
  public class Icon
  {
    [Key(0)]
    public string Shape { get; set; }

    [Key(1)]
    public string Color1 { get; set; }

    [Key(2)]
    public string Color2 { get; set; }
  }
}
