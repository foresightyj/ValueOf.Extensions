using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ValueOf.Extensions
{
    public static class ValueOfTypeExtensions
    {
        private static string valueOfNs = typeof(ValueOf<,>).Namespace!;
        private static string valueOfName = typeof(ValueOf<,>).Name!;

        public static bool IsValueOfType(this Type t, out Type? underlyingType)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            var b = t.BaseType;
            underlyingType = null;
            if (b == null) return false;

            if (b.Namespace != valueOfNs || b.Name != valueOfName) return false;
            var typeArgs = b.GenericTypeArguments;
            if (typeArgs == null || typeArgs.Length != 2)
            {
                throw new InvalidOperationException($"wrong ValueOf generic args count (expecting 2) in {b.FullName}");
            }

            underlyingType = typeArgs[0];
            if (typeArgs[1] != t)
            {
                throw new InvalidOperationException(
                    $"Expecting typeArgs[1] {typeArgs[1].FullName} be to {t.FullName} but isn't. Let me know if there are in fact valid use cases of this");
            }

            return true;
        }

        private static void configureValueOfTypeConverters(IEnumerable<Type> types)
        {
            foreach (var valueOfType in types)
            {
                if (valueOfType.IsValueOfType(out var ut))
                {
                    var converterType =
                        new TypeConverterAttribute(typeof(ValueOfTypeConverter<,>).MakeGenericType(ut!, valueOfType));
                    TypeDescriptor.AddAttributes(valueOfType, converterType);
                }
            }
        }

        public static void ConfigureValueOfTypeConverters(params Type[] types)
        {
            configureValueOfTypeConverters(types);
        }

        public static void ConfigureValueOfTypeConverters(params Assembly[] assemblies)
        {
            configureValueOfTypeConverters(assemblies.SelectMany(a => a.DefinedTypes).Select(t => t.AsType()));
        }
    }
}