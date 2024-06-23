using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace AddIn.Core.Helpers
{
  public static class WindowsFormsFileSource
  {

    public static string Text()
    {
      System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
      openFileDialog.Title = "Select a File";
      openFileDialog.Filter = "All Files (*.*)|*.*";
      if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        string selectedFileName = openFileDialog.FileName;
        var text = SerializationHelper.ReadTextFromFile(selectedFileName);
        return text;
      }

      return string.Empty;
    }


    public static T DeserializeFile<T>(string title = "Select a File") where T : class, new()
    {
      System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
      openFileDialog.Title = title;
      openFileDialog.Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|YAML Files (*.yaml)|*.yaml";
      if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        string selectedFileName = openFileDialog.FileName;
          var deserialized =  SerializationHelper.DeserializeFromFile<T>(selectedFileName);
          return deserialized;
      }

      return default(T);
    }


    public static void SerializeToFile<T>(T obj, string title = "Save File As")
    {
      System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      saveFileDialog.Title = title;
      saveFileDialog.Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|YAML Files (*.yaml)|*.yaml";
      if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        string selectedFileName = saveFileDialog.FileName;
        SerializationHelper.SerializeToFile(obj, selectedFileName);
      }
    }


  }
}
