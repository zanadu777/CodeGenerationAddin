using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core;

namespace AttributeHelperAddin.Commands
{
  public class ApplyAttributeSpecification
  {
    public CodeElementLocation TargetLocation { get; set; }
    public string AttributeName { get; set; }

    public Dictionary<string,string> SignatureArgumentPairs { get; set; }

  }
}
