using System.Collections.Generic;
using CodeModel;

public class MethodInfoData
{
    public string Name { get; set; }
    public string ReturnType { get; set; }
    public bool IsStatic { get; set; }
    public bool IsOverride { get; set; }
    public bool IsAbstract { get; set; }
    public List<AttributeInfoData> Attributes { get; set; }
    public List<ParameterInfoData> Parameters { get; set; }
}


