using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace AddIn.Core
{
  public  class CompleteCodeClass
  {

    public List<CodeClass> CodeClasses { get; set; } = new List<CodeClass>();

    public string FullName => CodeClasses.First().FullName;
    public string Name => CodeClasses.First().Name;
    
    public CompleteCodeClass(CodeClass codeClass)
    {
      CodeClasses.Add(codeClass);
    }

    public CompleteCodeClass( IEnumerable< CodeClass> codeClasses)
    {
      CodeClasses.AddRange(codeClasses);
    }
  }
}
