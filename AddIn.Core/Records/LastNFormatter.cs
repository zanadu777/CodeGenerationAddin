using System.Collections.Generic;
using AddIn.Core.Records;
using MessagePack;
using MessagePack.Formatters;

public class LastNFormatter<T> : IMessagePackFormatter<LastN<T>>
{
  public void Serialize(ref MessagePackWriter writer, LastN<T> value, MessagePackSerializerOptions options)
  {
    // Start an array in MessagePack format. The number should be equal to the number of properties.
    writer.WriteArrayHeader(2);

    // Serialize the Count property.
    writer.Write(value.Capacity);

    // Serialize the Items property.
    // You can use the options argument to get a formatter for the T type.
    var formatter = options.Resolver.GetFormatterWithVerify<List<T>>();
    formatter.Serialize(ref writer, new List<T>(value.Items), options);
  }

  public LastN<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
  {
    // Ensure the incoming MessagePack has the correct number of elements
    if (reader.ReadArrayHeader() != 2)
    {
      throw new MessagePackSerializationException("Expected an array of length 2 in the MessagePack.");
    }

    // Deserialize the Count property
    var Capacity = reader.ReadInt32();

    // Deserialize the Items property
    var formatter = options.Resolver.GetFormatterWithVerify<List<T>>();
    var items = formatter.Deserialize(ref reader, options);

    // Return a new LastN<T> object with the deserialized properties
    var lastN = new LastN<T>(Capacity);
    foreach (var item in items)
      lastN.Add(item);
    return lastN;
  }
}
