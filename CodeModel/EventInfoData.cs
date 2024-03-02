using System.Collections.Generic;
using CodeModel;

public class EventInfoData
{
    public string Name { get; set; }
    public string EventHandlerType { get; set; }
    public bool IsStatic { get; set; }
    public List<AttributeInfoData> Attributes { get; set; }
}
