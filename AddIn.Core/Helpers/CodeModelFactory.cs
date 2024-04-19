using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeModel.InfoData;
using EnvDTE;
using EnvDTE80;

namespace AddIn.Core.Helpers
{
  public class CodeModelFactory
  {
    public static Dictionary<string, ClassInfoData> KnownClasses { get; set; } = new Dictionary<string, ClassInfoData>();
 
    #region CodeClass2
    public ClassInfoData CreateClassInfoData(CodeClass2 codeClass)
    {
      var outputFileName = codeClass.ProjectItem.ContainingProject.Properties.Item("OutputFileName").Value.ToString();
      var assemblyName = Path.GetFileName(outputFileName);

      var classInfoData = new ClassInfoData
      {
        Name = codeClass.Name,
        FullName = codeClass.FullName,
        AssemblyName = assemblyName,
        BaseClassName = codeClass.Bases.Cast<CodeClass>().FirstOrDefault()?.FullName,
        BaseClassNames = codeClass.Bases.Cast<CodeClass>().Select(x=>x.FullName).ToList(),
        IsAbstract = codeClass.IsAbstract,
        IsGeneric = codeClass.IsGeneric,
        IsStatic = codeClass.IsShared,
        Access = codeClass.Access.ToString(),
        DocComment = codeClass.DocComment,
        Namespace = codeClass.Namespace?.FullName,
        Attributes = codeClass.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList(),
        Properties = codeClass.Members.OfType<CodeProperty2>()
          .Select(CreatePropertyInfoData)
          .ToList(),
        Events = codeClass.Members.OfType<CodeEvent>()
          .Select(CreateEventInfoData)
          .ToList(),
        Methods = codeClass.Members.OfType<CodeFunction>()
          .Select(CreateMethodInfoData)
          .ToList(),
        Fields = codeClass.Members.OfType<CodeVariable>()
          .Select(CreateFieldInfoData)
          .ToList()
      };


       
      return classInfoData;
    }


    public AttributeInfoData CreateAttributeInfoData(CodeAttribute2 attribute)
    {
      var attributeInfoData = new AttributeInfoData
      {
        Name = attribute.Name,
        Arguments = attribute.Arguments.OfType<CodeAttributeArgument>()
          .Select(arg => new AttributeArgumentInfoData { Name = arg.Name, Value = arg.Value })
          .ToList()
      };

      return attributeInfoData;
    }


    public PropertyInfoData CreatePropertyInfoData(CodeProperty2 property)
    {
      var propertyInfoData = new PropertyInfoData
      {
        Name = property.Name,
        FullName = property.FullName,
        Access = property.Access.ToString(),
        ReturnType = property.Type.AsString,
        IsStatic = property.IsShared,
        IsOverride = property.OverrideKind != vsCMOverrideKind.vsCMOverrideKindNone,
        Attributes = property.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return propertyInfoData;
    }

    public EventInfoData CreateEventInfoData(CodeEvent codeEvent)
    {
      var eventInfoData = new EventInfoData
      {
        Name = codeEvent.Name,
        EventHandlerType = codeEvent.Type.AsString,
        IsStatic = codeEvent.IsShared,
        Attributes = codeEvent.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return eventInfoData;
    }


    public MethodInfoData CreateMethodInfoData(CodeFunction codeFunction)
    {
      var methodInfoData = new MethodInfoData
      {
        Name = codeFunction.Name,
        ReturnType = codeFunction.Type.AsString,
        IsStatic = codeFunction.IsShared,
        IsAbstract = codeFunction.MustImplement,
        Attributes = codeFunction.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList(),
        Parameters = codeFunction.Parameters.Cast<CodeParameter2>()
          .Select(CreateParameterInfoData)
          .ToList()
      };

      if (codeFunction.FunctionKind == vsCMFunction.vsCMFunctionFunction)
      {
        var baseClass = ((CodeClass2)codeFunction.Parent).Bases.Cast<CodeClass>().FirstOrDefault();
        if (baseClass != null)
        {
          var baseMethod = baseClass.Members.OfType<CodeFunction>().FirstOrDefault(m => m.Name == codeFunction.Name);
          methodInfoData.IsOverride = baseMethod != null;
        }
      }

      return methodInfoData;
    }


    public ParameterInfoData CreateParameterInfoData(CodeParameter2 parameter)
    {
      var parameterInfoData = new ParameterInfoData
      {
        Name = parameter.Name,
        ParameterType = parameter.Type.AsString,
        IsOptional = !string.IsNullOrEmpty(parameter.DefaultValue),
        DefaultValue = parameter.DefaultValue,
        Attributes = parameter.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return parameterInfoData;
    }

    public FieldInfoData CreateFieldInfoData(CodeVariable codeVariable)
    {
      var fieldInfoData = new FieldInfoData
      {
        Name = codeVariable.Name,
        FieldType = codeVariable.Type.AsString,
        IsStatic = codeVariable.IsShared,
        Attributes = codeVariable.Attributes.Cast<CodeAttribute2>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return fieldInfoData;
    }


    #endregion

    #region Type (System.Type) via reflection

    public ClassInfoData CreateClassInfoData(Type type)
    {
      var classInfoData = new ClassInfoData
      {
        Name = type.Name,
        FullName = type.FullName,
        AssemblyName = type.Assembly.GetName().Name,
        BaseClassName = type.BaseType?.FullName,
        BaseClassNames = GetBaseClasses(type).Select(x => x.FullName).ToList(),
        IsAbstract = type.IsAbstract,
        IsGeneric = type.IsGenericType,
        IsStatic = type.IsAbstract && type.IsSealed, // In C#, a static class is declared as abstract and sealed
        Access = type.IsPublic ? "Public" : "NonPublic", // Simplified; doesn't account for protected, internal, etc.
        DocComment = "", // Not available from System.Type
        Namespace = type.Namespace,
        Attributes = Attribute.GetCustomAttributes(type)
          .Select(CreateAttributeInfoData)
          .ToList(),
        Properties = type.GetProperties()
          .Select(CreatePropertyInfoData)
          .ToList(),
        Events = type.GetEvents()
          .Select(CreateEventInfoData)
          .ToList(),
        Methods = type.GetMethods()
          .Select(CreateMethodInfoData)
          .ToList(),
        Fields = type.GetFields()
          .Select(CreateFieldInfoData)
          .ToList()
      };

      return classInfoData;
    }

    private IEnumerable<Type> GetBaseClasses(Type type)
    {
      if (type.BaseType == null)
      {
        return Enumerable.Empty<Type>();
      }

      return Enumerable.Repeat(type.BaseType, 1).Concat(GetBaseClasses(type.BaseType));
    }

    public AttributeArgumentInfoData CreateAttributeArgumentInfoData(PropertyInfo property, object attribute)
    {
      var argumentInfoData = new AttributeArgumentInfoData
      {
        Name = property.Name,
        Value = property.GetValue(attribute)?.ToString()
      };

      return argumentInfoData;
    }

    public AttributeInfoData CreateAttributeInfoData(Attribute attribute)
    {
      var attributeInfoData = new AttributeInfoData
      {
        Name = attribute.GetType().Name,
        Arguments = attribute.GetType().GetProperties()
          .Select(prop => CreateAttributeArgumentInfoData(prop, attribute))
          .ToList()
      };

      return attributeInfoData;
    }

    public PropertyInfoData CreatePropertyInfoData(PropertyInfo property)
    {
      var propertyInfoData = new PropertyInfoData
      {
        Name = property.Name,
        ReturnType = property.PropertyType.Name,
        IsStatic = property.GetGetMethod().IsStatic,
        IsOverride = property.GetMethod.GetBaseDefinition().DeclaringType != property.GetMethod.DeclaringType,
        Attributes = property.GetCustomAttributes(true)
          .OfType<Attribute>()
          .Select(CreateAttributeInfoData)
          .ToList(),
      };

      return propertyInfoData;
    }

    public EventInfoData CreateEventInfoData(EventInfo eventInfo)
    {
      var eventInfoData = new EventInfoData
      {
        Name = eventInfo.Name,
        EventHandlerType = eventInfo.EventHandlerType.Name,
        IsStatic = eventInfo.AddMethod.IsStatic,
        Attributes = eventInfo.GetCustomAttributes(true)
          .OfType<Attribute>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return eventInfoData;
    }

    public MethodInfoData CreateMethodInfoData(MethodInfo method)
    {
      var methodInfoData = new MethodInfoData
      {
        Name = method.Name,
        ReturnType = method.ReturnType.Name,
        IsStatic = method.IsStatic,
        Attributes = method.GetCustomAttributes(true)
          .OfType<Attribute>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return methodInfoData;
    }

    public FieldInfoData CreateFieldInfoData(FieldInfo field)
    {
      var fieldInfoData = new FieldInfoData
      {
        Name = field.Name,
        FieldType = field.FieldType.Name,
        IsStatic = field.IsStatic,
        Attributes = field.GetCustomAttributes(true)
          .OfType<Attribute>()
          .Select(CreateAttributeInfoData)
          .ToList()
      };

      return fieldInfoData;
    }

    #endregion
  }
}
