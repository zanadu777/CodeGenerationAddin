using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Core.Reflection
{
  public static  class TypeExtensions
  {
    public static List<Type> InheritanceChain(Type type)
    {
      var ancestors = new List<Type> {type};

      while (type.BaseType != null)
      {
        ancestors.Add(type.BaseType);
        type = type.BaseType;
      }

      return ancestors;
    }
  }
}
