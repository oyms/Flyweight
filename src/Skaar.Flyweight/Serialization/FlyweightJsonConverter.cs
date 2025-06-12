using System.Text.Json;
using System.Text.Json.Serialization;
using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight.Serialization;

public class FlyweightJsonConverter<T> : JsonConverter<T> where T:IFlyweightFactory<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? default : T.Get(value);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}