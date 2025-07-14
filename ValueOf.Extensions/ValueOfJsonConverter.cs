using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueOf.Extensions
{
    public sealed class ValueOfJsonConverter<TU, T> : JsonConverter<T> where T : ValueOf<TU, T>, new()
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = JsonSerializer.Deserialize<TU>(ref reader, options);
            return ValueOf<TU, T>.From(val!);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}