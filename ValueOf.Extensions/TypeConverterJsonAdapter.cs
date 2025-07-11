using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions;

public class TypeConverterJsonAdapter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = TypeDescriptor.GetConverter(typeToConvert);
        var text = reader.GetString();
        return (T)converter.ConvertFromString(text!)!;
    }

    public override void Write(Utf8JsonWriter writer, T objectToWrite, JsonSerializerOptions options)
    {
        var converter = TypeDescriptor.GetConverter(objectToWrite!);
        var text = converter.ConvertToString(objectToWrite);
        writer.WriteStringValue(text);
    }

    public override bool CanConvert(Type typeToConvert)
    {
        var hasConverter = typeToConvert.IsDefined(typeof(TypeConverterAttribute), true);
        return hasConverter;
    }
}