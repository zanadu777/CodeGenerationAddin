using Microsoft.Win32;  

namespace AddIn.Core.Helpers
{
  public static class WpfFileSource
  {
    public static string Text()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select a File";
      openFileDialog.Filter = "All Files (*.*)|*.*";
      if (openFileDialog.ShowDialog() == true) // WPF dialog returns nullable bool
      {
        string selectedFileName = openFileDialog.FileName;
        var text = SerializationHelper.ReadTextFromFile(selectedFileName);
        return text;
      }
      return string.Empty;
    }

    public static T DeserializeFile<T>(string title = "Select a File") where T : class, new()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = title;
      openFileDialog.Filter = "JSON Files (*.json)|*.json|TOML Files (*.toml)|*.toml|XML Files (*.xml)|*.xml|YAML Files (*.yaml)|*.yaml";
      if (openFileDialog.ShowDialog() == true)
      {
        string selectedFileName = openFileDialog.FileName;
        return SerializationHelper.DeserializeFromFile<T>(selectedFileName);
      }

      return default(T);
    }

    public static void SerializeToFile<T>(T obj, string title = "Save File As")
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Title = title;
      saveFileDialog.Filter = "JSON Files (*.json)|*.json|TOML Files (*.toml)|*.toml|XML Files (*.xml)|*.xml|YAML Files (*.yaml)|*.yaml";
      if (saveFileDialog.ShowDialog() == true)
      {
        string selectedFileName = saveFileDialog.FileName;
        SerializationHelper.SerializeToFile(obj, selectedFileName);
      }
    }
  }
}