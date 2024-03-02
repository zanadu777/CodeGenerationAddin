using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeModel
{
  public class ClassInfoData
  {
    public List<AttributeInfoData> Attributes { get; set; } = new List<AttributeInfoData>();
    public List<PropertyInfoData> Properties { get; set; } = new List<PropertyInfoData>();
    public List<EventInfoData> Events { get; set; } = new List<EventInfoData>();

    public List<MethodInfoData> Methods { get; set; } = new List<MethodInfoData>();

    public List<FieldInfoData> Fields { get; set; } = new List<FieldInfoData>();

    public string Name { get; set; }
    public string FullName { get; set; }

    public string AssemblyName { get; set; }
    public string BaseClassName { get; set; }
    public List<string> BaseClassNames { get; set; } = new List<string>();

    public bool IsAbstract { get; set; }
    public bool IsGeneric { get; set; }
    public bool IsStatic { get; set; }
    public string Access { get; set; }
    public string DocComment { get; set; }
    public string Namespace { get; set; }
    
  }
}
