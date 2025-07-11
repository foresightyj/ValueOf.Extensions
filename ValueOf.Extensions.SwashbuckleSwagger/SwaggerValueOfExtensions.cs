using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwaggerTypeMap = System.Collections.Generic.IReadOnlyDictionary<System.Type, (string Type, string? Format)>;

namespace ValueOf.Extensions.SwashbuckleSwagger
{
    public static class SwaggerValueOfExtensions
    {
        private static readonly SwaggerTypeMap DefaultSwaggerTypeMap =
            new Dictionary<Type, (string Type, string? Format)>
            {
                { typeof(string), ("string", null) },
                { typeof(bool), ("boolean", null) },
                { typeof(int), ("integer", "int32") },
                { typeof(long), ("integer", "int64") },
                { typeof(float), ("number", "float") },
                { typeof(double), ("number", "double") },
                { typeof(decimal), ("number", "double") },
                { typeof(DateTime), ("string", "date-time") },
                { typeof(Guid), ("string", "uuid") },
                { typeof(object), ("object", null) },
            };

        private static (string Type, string? Format) lookupMappedType(Type type, SwaggerTypeMap? typeMapOverride = null)
        {
            if (typeMapOverride == null)
            {
                return DefaultSwaggerTypeMap.TryGetValue(type, out var val)
                    ? val
                    : throw new InvalidOperationException(
                        $"failed to lookup mapped type & format for type: {type.FullName}");
            }

            return typeMapOverride.TryGetValue(type, out var val2) ? val2 : lookupMappedType(type, null);
        }

        public static void MapValueOfTypes(this SwaggerGenOptions opts, IEnumerable<Type> valueOfTypes,
            SwaggerTypeMap? typeMapOverride = null)
        {
            if (opts == null) throw new ArgumentNullException(nameof(opts));
            if (valueOfTypes == null) throw new ArgumentNullException(nameof(valueOfTypes));
            var valueOfTypesPairs = (from t in valueOfTypes
                    let u = t.IsValueOfType(out var underlyingType) ? underlyingType : null
                    where u != null
                    select (t, u)
                );

            foreach (var (t, underlyingType) in valueOfTypesPairs)
            {
                if (!t.IsDefined(typeof(TypeConverterAttribute), false)) continue;
                opts.MapType(t, () =>
                {
                    var (type, format) = lookupMappedType(underlyingType, typeMapOverride);
                    return new OpenApiSchema
                    {
                        Type = type,
                        Format = format,
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["x-valueof-type"] = new OpenApiObject
                            {
                                ["name"] = new OpenApiString(t.Name),
                                ["full-name"] = new OpenApiString(t.FullName),
                            }
                        }
                    };
                });
            }
        }

        public static void MapValueOfTypesInAssemblies(this SwaggerGenOptions opts, params Assembly[] assemblies)
        {
            if (opts == null) throw new ArgumentNullException(nameof(opts));
            var valueOfTypes = from a in assemblies
                from t in a.DefinedTypes.Select(t => t.AsType())
                select t;
            MapValueOfTypes(opts, valueOfTypes);
        }
    }
}