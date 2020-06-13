using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace PetDoctor.Infrastructure
{
    public static class SerializationExtensions
    {
        public static string ToJson(this object @this)
        {
            return JsonConvert.SerializeObject(@this, Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );
        }

        public static T FromJson<T>(this string @this)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(@this, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            catch (JsonReaderException)
            {
                throw new FormatException($"Input string is not a valid representation of {typeof(T)}");
            }
        }
    }
}
