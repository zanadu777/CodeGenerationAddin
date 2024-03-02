using System.IO;
using System.IO.IsolatedStorage;

namespace CodeAddIn.Helpers
{
  public static class IsolatedStorageHelper
  {

    public static void WriteToIsolatedStorage(string fileName, string content)
    {
      using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly())
      {
        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, isolatedStorage))
        {
          using (StreamWriter writer = new StreamWriter(stream))
          {
            writer.Write(content);
          }
        }
      }
    }

    public static string ReadFromIsolatedStorage(string fileName)
    {
      using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly())
      {
        if (!isolatedStorage.FileExists(fileName))
        {
          return null;
        }

        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, isolatedStorage))
        {
          using (StreamReader reader = new StreamReader(stream))
          {
            return reader.ReadToEnd();
          }
        }
      }
    }
  }
}