using System.Collections.Generic;

namespace CodeModel.InfoData
{
  public class EventInfoData
  {
    public string Name { get; set; }
    public string EventHandlerType { get; set; }
    public bool IsStatic { get; set; }
    public List<AttributeInfoData> Attributes { get; set; }
  }
}
