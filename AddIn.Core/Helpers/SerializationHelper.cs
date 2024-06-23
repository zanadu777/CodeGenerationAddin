using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace AddIn.Core.Helpers
{
  public static class SerializationHelper
  {
    public static string ReadTextFromFile(string filePath)
    {
      return File.ReadAllText(filePath);
    }

    public static T DeserializeFromFile<T>(string filePath) where T : class, new()
    {
      string fileExtension = Path.GetExtension(filePath).ToLower();
      switch (fileExtension)
      {
        case ".json":
          using (StreamReader file = File.OpenText(filePath))
          {
            JsonSerializer serializer = new JsonSerializer();
            return (T)serializer.Deserialize(file, typeof(T));
          }
        case ".xml":
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
          using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
          {
            return (T)xmlSerializer.Deserialize(fileStream);
          }
        case ".yaml":
          var yamlDeserializer = new Deserializer();
          using (var reader = new StreamReader(filePath))
          {
            return yamlDeserializer.Deserialize<T>(reader);
          }
        default:
          throw new InvalidOperationException("Unsupported file type.");
      }
    }

    public static void SerializeToFile<T>(T obj, string filePath)
    {
      string fileExtension = Path.GetExtension(filePath).ToLower();
      switch (fileExtension)
      {
        case ".json":
          using (StreamWriter file = File.CreateText(filePath))
          {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, obj);
          }
          break;
        case ".xml":
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
          using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
          {
            xmlSerializer.Serialize(fileStream, obj);
          }
          break;
        case ".yaml":
          var yamlSerializer = new Serializer();
          using (var writer = new StreamWriter(filePath))
          {
            yamlSerializer.Serialize(writer, obj);
          }
          break;
        default:
          throw new InvalidOperationException("Unsupported file type.");
      }
    }
  }
}
