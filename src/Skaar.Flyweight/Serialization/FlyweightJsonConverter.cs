using System.Text.Json;
using System.Text.Json.Serialization;
using Skaar.Flyweight.Contracts;

namespace Skaar.Flyweight.Serialization;

/// <summary>
/// A JSON converter that serializes and deserializes flyweight objects.
/// </summary>
public class FlyweightJsonConverter<T> : JsonConverter<T> where T:IFlyweightFactory<T, string>
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

/// <summary>
/// A JSON converter that serializes and deserializes flyweight objects.
/// </summary>
public class FlyweightJsonConverter<T, TInner> : JsonConverter<T> where T:FlyweightBase<T, TInner>, IFlyweightFactory<T, TInner> where TInner : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = GetInnerConverter(options);
        var value = converter.Read(ref reader, typeof(TInner), options);
        return value is null ? default : T.Get(value);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var converter = GetInnerConverter(options);
        converter.Write(writer, value, options);
    }

    private JsonConverter<TInner> GetInnerConverter(JsonSerializerOptions options)
    {
        var converter = options.GetConverter(typeof(TInner)) as JsonConverter<TInner>;
        if(converter is null)
        {
            throw new JsonException($"No converter found for type {typeof(TInner).FullName}");
        }

        return converter;
    }
}