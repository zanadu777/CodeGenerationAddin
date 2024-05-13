using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Records
{
  public class SearchResult
  {
    public string Code { get; set; }
    public string File { get; set; }
    public int Line { get; set; }
    public int Col { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string Project { get; set; }
  };
}
