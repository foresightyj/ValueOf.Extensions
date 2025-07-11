using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions
{
    public class ValueOfTypeConverterJsonAdapterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsValueOfType(out _)) return false;
            var hasConverter = typeToConvert.IsDefined(typeof(TypeConverterAttribute), false);
            return hasConverter;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!typeToConvert.IsValueOfType(out var underlyingType))
            {
                throw new InvalidOperationException($"invalid ValueOf type received: {typeToConvert.FullName}");
            }

            var converterType =
                typeof(ValueOfTypeConverterJsonAdapter<,>).MakeGenericType(underlyingType, typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}