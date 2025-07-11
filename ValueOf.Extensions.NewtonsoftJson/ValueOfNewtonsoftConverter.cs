using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace ValueOf.Extensions.NewtonsoftJson
{
    public sealed class ValueOfNewtonsoftConverter : JsonConverter
    {
        private readonly ConcurrentDictionary<Type, (Type underlyingType, Func<object, object> wrapValueOf,
            Func<object, object> unwrapValueOf)> _cache =
            new ConcurrentDictionary<Type, (Type underlyingType, Func<object, object> wrapValueOf, Func<object, object>
                unwrapValueOf)>();

        public override bool CanConvert(Type objectType) => objectType.IsValueOfType(out _);

        private static (Type underlyingType, Func<object, object> wrapValueOf, Func<object, object> unwrapValueOf)
            cacheValueFactory(Type valueOfType)
        {
            if (!valueOfType.IsValueOfType(out var underlyingType))
            {
                throw new InvalidOperationException($"{valueOfType.FullName} must be a ValueOf type");
            }

            Debug.Assert(underlyingType != null, nameof(underlyingType) + " != null");


            return (underlyingType, makeWrapValueOfFunc(valueOfType, underlyingType),
                makeUnwrapValueOfFunc(valueOfType, underlyingType));
        }

        private static Func<object, object> makeWrapValueOfFunc(Type valueOfType, Type underlyingType)
        {
            //build a Func<object, object> dynamically where the input object of underlyingType and the returned object is of valueOfType
            var input = Expression.Parameter(typeof(object), "input");
            Expression converted = Expression.Convert(input, underlyingType);

            var fromMethod = valueOfType.GetMethod("From",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fromMethod == null)
            {
                throw new InvalidOperationException($"From method not found in type: {valueOfType.FullName}");
            }

            var call = Expression.Call(null, fromMethod, converted);
            var output = Expression.Convert(call, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(output, input);
            var wrapValueOf = lambda.Compile();
            return wrapValueOf;
        }

        private static Func<object, object> makeUnwrapValueOfFunc(Type valueOfType, Type underlyingType)
        {
            //build a Func<object, object> dynamically where the input object of underlyingType and the returned object is of valueOfType
            var input = Expression.Parameter(typeof(object), "input");
            Expression converted = Expression.Convert(input, valueOfType);
            var valueProp = valueOfType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            Debug.Assert(valueProp != null, nameof(valueProp) + " != null");
            var value = Expression.Call(converted, valueProp.GetGetMethod());
            var output = Expression.Convert(value, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(output, input);
            var unwrapValueOf = lambda.Compile();
            return unwrapValueOf;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var (underlyingType, wrapValueOf, _) = _cache.GetOrAdd(objectType, cacheValueFactory);
            var value = serializer.Deserialize(reader, underlyingType);
            return wrapValueOf(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Debug.Assert(value != null, nameof(value) + " != null");
            var (underlyingType, _, unwrapValueOf) = _cache.GetOrAdd(value.GetType(), cacheValueFactory);
            var unwrapped = unwrapValueOf(value);
            serializer.Serialize(writer, unwrapped);
        }
    }
}