using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PetDoctor.Infrastructure;

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
            var deserialized = JsonConvert.DeserializeObject<T>(@this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            if (deserialized == null)
            {
                throw new JsonReaderException();
            }

            return deserialized;
        }
        catch (JsonReaderException)
        {
            throw new FormatException($"Input string is not a valid representation of {typeof(T)}");
        }
    }
}