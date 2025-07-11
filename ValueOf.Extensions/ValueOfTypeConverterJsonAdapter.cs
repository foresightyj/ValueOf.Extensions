using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions
{
    public class ValueOfTypeConverterJsonAdapter<TU, T> : JsonConverter<T> where T : ValueOf<TU, T>, new()
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = TypeDescriptor.GetConverter(typeToConvert);
            var val = JsonSerializer.Deserialize<TU>(ref reader, options);
            var isDefault = EqualityComparer<TU>.Default.Equals(val!, default!);
            if (isDefault)
            {
                return null;
            }

            return (T)converter.ConvertFrom(val)!;
        }

        public override void Write(Utf8JsonWriter writer, T objectToWrite, JsonSerializerOptions options)
        {
            var converter = TypeDescriptor.GetConverter(objectToWrite!);
            var typeofU = typeof(TU);
            var typeofT = typeof(T);
            Console.WriteLine($"{typeofU.Name} vs {typeofT.Name}");
            var val = converter.ConvertTo(objectToWrite!, typeof(TU));
            JsonSerializer.Serialize(writer, val, options);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            var hasConverter = typeToConvert.IsDefined(typeof(TypeConverterAttribute), false);
            return hasConverter;
        }
    }
}