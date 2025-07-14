using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions
{
    public sealed class ValueOfJsonAdapterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsValueOfType(out _);

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!typeToConvert.IsValueOfType(out var ut))
                throw new InvalidOperationException($"typeToConvert: {typeToConvert.FullName} is not a ValueOf type");
            var converterType = typeof(ValueOfJsonConverter<,>).MakeGenericType(ut, typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}