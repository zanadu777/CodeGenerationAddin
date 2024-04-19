using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace AddIn.Core.Extensions
{
  public static class CodeClass2Extensions
  {

    public static void GetDetails(this CodeClass2 codeClass)
    {
      string name = codeClass.Name; // The name of the class
      string fullName = codeClass.FullName; // The full name of the class, including namespace
      CodeElements members = codeClass.Members; // The members (methods, properties, etc.) of the class
      foreach (CodeElement2 member in members)
      {


      }
      bool isAbstract = codeClass.IsAbstract; // Whether the class is abstract
      bool isGeneric = codeClass.IsGeneric; // Whether the class is generic
     //bool isPartial = codeClass.IsPartial; // Whether the class is partial
    }

    public static List<CodeProperty2> GetProperties(this CodeClass2 codeClass)
    {
      var properties = new List<CodeProperty2>();

      foreach (CodeElement member in codeClass.Members)
      {
        if (member.Kind == vsCMElement.vsCMElementProperty)
        {
          var property = (CodeProperty2)member;
          var name = property.Name;
          var attributes = property.Attributes;
          properties.Add(property);
        }
      }

      return properties;
    }
    public static List<CodeFunction2> GetMethods(this CodeClass2 codeClass)
    {
      var methods = new List<CodeFunction2>();

      foreach (CodeElement member in codeClass.Members)
      {
        if (member.Kind == vsCMElement.vsCMElementFunction)
        {
          var method = (CodeFunction2)member;
          methods.Add(method);
        }
      }

      return methods;
    }
    public static List<CodeVariable> GetFields(this CodeClass2 codeClass)
    {
      var fields = new List<CodeVariable>();

      foreach (CodeElement member in codeClass.Members)
      {
        if (member.Kind == vsCMElement.vsCMElementVariable)
        {
          var field = (CodeVariable)member;
          fields.Add(field);
        }
      }

      return fields;
    }

    public static List<CodeEvent> GetEvents(this CodeClass2 codeClass)
    {
      var events = new List<CodeEvent>();

      foreach (CodeElement member in codeClass.Members)
      {
        if (member.Kind == vsCMElement.vsCMElementEvent)
        {
          var eventMember = (CodeEvent)member;
          events.Add(eventMember);
        }
      }

      return events;
    }
  }
}
