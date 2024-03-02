using System.Collections.Generic;

namespace CodeModel.InfoData
{
  public class FieldInfoData
  {
    public string Name { get; set; }
    public string FieldType { get; set; }
    public bool IsStatic { get; set; }
    public List<AttributeInfoData> Attributes { get; set; }
  }
}
