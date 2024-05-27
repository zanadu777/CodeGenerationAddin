using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using MessagePack;

namespace AddIn.Core.Helpers
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

    public static async Task SerializeToIsolatedStorageAsync<T>(T obj, string filePath)
    {
      using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain())
      {
        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Create, storage))
        {
          await MessagePackSerializer.SerializeAsync(stream, obj);
        }
      }
    }

    public static async Task<T> DeserializeFromIsolatedStorageAsync<T>(string filePath)
    {
      using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain())
      {
        if (!storage.FileExists(filePath))
        {
          return default(T);
        }

        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Open, storage))
        {
          if (stream.Length == 0)
            return default(T);

          return await MessagePackSerializer.DeserializeAsync<T>(stream);
        }
      }
    }
  }


}