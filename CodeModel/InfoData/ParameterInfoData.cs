using System.Collections.Generic;

namespace CodeModel.InfoData
{
  public class ParameterInfoData
  {
    public string Name { get; set; }
    public string ParameterType { get; set; }
    public bool IsOptional { get; set; }
    public object DefaultValue { get; set; }
    public List<AttributeInfoData> Attributes { get; set; }
  }
}
