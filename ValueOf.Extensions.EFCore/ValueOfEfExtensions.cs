using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ValueOf.Extensions.EFCore;

public static class ValueOfEfExtensions
{
    public static void ConfigureValueOfConversions(this ModelConfigurationBuilder b, params Type[] types)
    {
        _configureValueOfEfConversions(b, types);
    }

    public static void ConfigureValueOfConversions(this ModelConfigurationBuilder b, params Assembly[] assemblies)
    {
        _configureValueOfEfConversions(b, assemblies.SelectMany(a => a.DefinedTypes).Select(t => t.AsType()));
    }

    private static void _addEfConversionImpl<TU, T>(ModelConfigurationBuilder b) where T : ValueOf<TU, T>, new()
    {
        b.Properties<T>().HaveConversion<ValueOfConverter<TU, T>>();
    }

    private static readonly MethodInfo _addEfConversionMethod =
        typeof(ValueOfEfExtensions).GetMethod(nameof(_addEfConversionImpl),
            BindingFlags.Static | BindingFlags.NonPublic) ??
        throw new InvalidOperationException("failed to find _addEfConversionMethod method");

    private static void _addTypeHandler(ModelConfigurationBuilder b, Type valueOfType, Type ut)
    {
        var gm = _addEfConversionMethod.MakeGenericMethod(ut, valueOfType);
        gm.Invoke(null, new[] { b });
    }

    private static void _configureValueOfEfConversions(ModelConfigurationBuilder b, IEnumerable<Type> types)
    {
        foreach (var valueOfType in types)
        {
            if (valueOfType.IsValueOfType(out var ut))
            {
                _addTypeHandler(b, valueOfType, ut);
            }
        }
    }
}