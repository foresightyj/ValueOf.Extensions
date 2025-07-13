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
        private static class WrapperUnwrapper<TValue, TThis> where TThis : ValueOf<TValue, TThis>, new()
        {
            public static object Wrap(object obj)
            {
                var val = (TValue)obj;
                return ValueOf<TValue, TThis>.From(val);
            }

            public static object Unwrap(object obj)
            {
                var val = (TThis)obj;
                return val.Value;
            }
        }

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

            var wrapperType = typeof(WrapperUnwrapper<,>).MakeGenericType(underlyingType, valueOfType);

            var wrapMethodInfo = wrapperType.GetMethod("Wrap", BindingFlags.Static | BindingFlags.Public);
            Debug.Assert(wrapMethodInfo != null, nameof(wrapMethodInfo) + " != null");

            var unwrapMethodInfo = wrapperType.GetMethod("Unwrap", BindingFlags.Static | BindingFlags.Public);
            Debug.Assert(unwrapMethodInfo != null, nameof(unwrapMethodInfo) + " != null");
            var wrapMethod =
                (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), wrapMethodInfo);
            var unwrapMethod =
                (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), unwrapMethodInfo);

            return (underlyingType, wrapMethod, unwrapMethod);
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