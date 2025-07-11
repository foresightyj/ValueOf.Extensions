using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions;

public class TypeConverterJsonAdapterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var hasConverter = typeToConvert.IsDefined(typeof(TypeConverterAttribute), true);
        return hasConverter;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(TypeConverterJsonAdapter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}