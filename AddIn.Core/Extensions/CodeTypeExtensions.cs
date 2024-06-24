using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace AddIn.Core.Extensions
{
  public static class CodeTypeExtensions
  {
    public static void SetAttributeOfMembers(this CodeType codeType, string attributeName, Dictionary<string, string> signatureArgumentPairs, bool removeIfNotInSignatureArgumentPairs = true)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (codeType == null)
        throw new ArgumentNullException(nameof(codeType));
      if (string.IsNullOrEmpty(attributeName))
        throw new ArgumentException("Attribute name cannot be null or empty.", nameof(attributeName));
      if (signatureArgumentPairs == null)
        throw new ArgumentNullException(nameof(signatureArgumentPairs));

      var allSignatures = new HashSet<string>(signatureArgumentPairs.Keys);

      foreach (CodeElement member in codeType.Members)
      {
        string signature = string.Empty;
        string argumentValue = string.Empty;

        switch (member)
        {
          case CodeFunction codeFunction:
            signature = GetMethodSignature(codeFunction);
            break;
          case CodeProperty codeProperty:
            signature = codeProperty.Name;
            break;
          case CodeEvent codeEvent:
            signature = codeEvent.Name;
            break;
          case CodeVariable codeVariable when codeVariable.IsShared:
            signature = codeVariable.Name;
            break;
          case CodeDelegate codeDelegate:
            signature = codeDelegate.Name;
            break;
        }

        if (!string.IsNullOrEmpty(signature))
        {
          if (signatureArgumentPairs.TryGetValue(signature, out argumentValue))
          {
            AddAttributeToMember(member, attributeName, argumentValue);
          }
          else if (removeIfNotInSignatureArgumentPairs)
          {
            RemoveAttributeFromMember(member, attributeName);
          }
        }

        // Optionally, remove the signature from the set to track which signatures were processed
        allSignatures.Remove(signature);
      }
    }

    private static string GetMethodSignature(CodeFunction codeFunction)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var parameters = codeFunction.Parameters.Cast<CodeParameter>()
        .Select(param => $"{param.Type.AsString} {param.Name}")
        .Aggregate((current, next) => current + ", " + next);

      return $"{codeFunction.Name}({parameters})";
    }


    private static void AddAttributeToMember(CodeElement member, string attributeName, string argumentValue, bool isExistingReplaced = true)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      CodeElements attributes = null;

      // Preprocess the argumentValue to correctly format it for the attribute
      string formattedArgumentValue = FormatArgumentValue(argumentValue);

      switch (member)
      {
        case CodeFunction codeFunction:
          attributes = codeFunction.Attributes;
          break;
        case CodeProperty codeProperty:
          attributes = codeProperty.Attributes;
          break;
        case CodeClass codeClass:
          attributes = codeClass.Attributes;
          break;
        case CodeVariable codeVariable:
          attributes = codeVariable.Attributes;
          break;
        case CodeEvent codeEvent:
          attributes = codeEvent.Attributes;
          break;
        case CodeDelegate codeDelegate:
          attributes = codeDelegate.Attributes;
          break;
      }

      if (attributes != null)
      {
        CodeAttribute existingAttribute = attributes.Cast<CodeAttribute>()
            .FirstOrDefault(attr => attr.Name == attributeName);

        if (existingAttribute != null)
        {
          if (!isExistingReplaced)
          {
            return; // Skip adding or replacing the attribute if it already exists and replacing is not allowed
          }
          else
          {
            // Replace the existing attribute's value
            existingAttribute.Value = formattedArgumentValue;
            return;
          }
        }

        // Construct the attribute text to insert if no existing attribute was found or replacing is allowed
        string attributeText = $"[{attributeName}({formattedArgumentValue})]";

        // The EditPoint object is used to insert text into the code file
        TextPoint startPoint = member.GetStartPoint(vsCMPart.vsCMPartHeader);
        EditPoint editPoint = startPoint.CreateEditPoint();
        editPoint.Insert(attributeText + Environment.NewLine);
      }
    }


    private static void RemoveAttributeFromMember(CodeElement member, string attributeName)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      CodeElements attributes = null;

      // Similar switch statement as in AddAttributeToMember to get the attributes collection
      switch (member)
      {
        case CodeFunction codeFunction:
          attributes = codeFunction.Attributes;
          break;
        case CodeProperty codeProperty:
          attributes = codeProperty.Attributes;
          break;
        case CodeClass codeClass:
          attributes = codeClass.Attributes;
          break;
        case CodeVariable codeVariable:
          attributes = codeVariable.Attributes;
          break;
        case CodeEvent codeEvent:
          attributes = codeEvent.Attributes;
          break;
        case CodeDelegate codeDelegate:
          attributes = codeDelegate.Attributes;
          break;
      }

      if (attributes != null)
      {
        CodeAttribute attributeToRemove = attributes.Cast<CodeAttribute>().FirstOrDefault(attr => attr.Name == attributeName);
        if (attributeToRemove != null)
        {
          attributeToRemove.Delete();
        }
      }
    }

    private static string FormatArgumentValue(string argumentValue)
    {
      // Split the argumentValue by commas not within quotes
      var arguments = argumentValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(arg => arg.Trim())
        .Select(arg =>
        {
          // Check if the argument is a number
          if (double.TryParse(arg, out _))
          {
            return arg; // Return numbers unquoted
          }
          else
          {
            // Ensure strings are properly quoted
            return $"\"{arg.Trim('"')}\"";
          }
        });

      return string.Join(", ", arguments);
    }
  }
}
