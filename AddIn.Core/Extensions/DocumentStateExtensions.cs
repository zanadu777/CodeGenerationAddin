using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using AddIn.Core.Helpers;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
 

namespace AddIn.Core.Extensions
{
  public static class DocumentStateExtensions
  {
    public static DocumentState GetDocumentState(this DTE2 dte)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      return new DocumentState
      {
        ActiveDocument = PathHelper.RelativePath(dte.Solution.FullName, dte.ActiveDocument.FullName),
        OpenDocuments = dte.GetOpenDocumentRelativePaths()
      };
    }

    public static List<string> GetOpenDocumentRelativePaths(this DTE2 dte)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var names = new List<string>();
      string solutionPath = Path.GetDirectoryName(dte.Solution.FullName);

      foreach (EnvDTE.Document doc in dte.Documents)
      {
        string relativePath = PathHelper.RelativePath(solutionPath, doc.FullName);
        names.Add(relativePath);
      }
      return names;
    }

    public static string ToJson(this DocumentState documentState)
    {
      return JsonConvert.SerializeObject(documentState, Formatting.Indented);
    }


    public static void RestoreDocumentState(this DTE dte, DocumentState documentState)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      string solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName);

      // Open all documents
      foreach (string relativeFilePath in documentState.OpenDocuments)
      {
        string absoluteFilePath = Path.Combine(solutionDirectory, relativeFilePath);
        dte.ItemOperations.OpenFile(absoluteFilePath, Constants.vsViewKindPrimary);
      }

      // Activate the active document
      if (!string.IsNullOrEmpty(documentState.ActiveDocument))
      {
        string absoluteFilePath = Path.Combine(solutionDirectory, documentState.ActiveDocument);
        Window activeWindow = dte.ItemOperations.OpenFile(absoluteFilePath, Constants.vsViewKindPrimary);
        Document activeDocument = activeWindow.Document;
        activeDocument.Activate();
      }
    }

  }
}
