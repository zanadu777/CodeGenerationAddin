using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core
{
  public  class DocumentState
  {
    public string ActiveDocument { get; set; }
    public List<string> OpenDocuments { get; set; }
  }
}
