using System.Collections.Generic;
using CodeModel;

public class PropertyInfoData
{
  public string Name { get; set; }
  public string FullName { get; set; }
  public string Access { get; set; }
  public string ReturnType { get; set; }
  public bool IsStatic { get; set; }
  public bool IsOverride { get; set; }
  public List<AttributeInfoData> Attributes { get; set; }
}
