using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AddIn.Core.Extensions
{
  public static class JsonExtensions
  {
    public static T DeserializeJson<T>(this string json)
    {
      return JsonConvert.DeserializeObject<T>(json);
    }
  }
}
