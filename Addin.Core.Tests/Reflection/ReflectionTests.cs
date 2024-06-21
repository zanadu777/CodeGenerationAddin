using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addin.Core.Tests.Reflection
{
  [TestClass]
  public class ReflectionTests
  {
    [TestMethod]
    public void InheritanceChainTests()
    {
      var t = typeof(ExtraNice);
      var b = t.BaseType;
      var text = VsName(b);
      text.Should().Be("GenericClass<NiceClass>");
    }


    public string VsName(Type t)
    {
      if (!t.IsGenericType)
        return t.Name;

      var genericTypeName = t.Name.Substring(0, t.Name.IndexOf('`'));
      var genericArgs = string.Join(",", t.GetGenericArguments()
        .Select(ta => VsName(ta)).ToArray());
      return $"{genericTypeName}<{genericArgs}>";
    }


    public class NiceClass
    {

    }

    public class GenericClass<T>
    {

    }

    public class ExtraNice : GenericClass<NiceClass>
    {

    }
  }
}
