using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Dapper;

namespace ValueOf.Extensions.Dapper
{
    public static class ValueOfDapperExtensions
    {
        private static void _addTypeHandlerImpl<TU, T>() where T : ValueOf<TU, T>, new()
        {
            SqlMapper.AddTypeHandler(new ValueOfDapperTypeHandler<TU, T>());
        }

        private static readonly MethodInfo _addTypeHandlerMethod =
            typeof(ValueOfDapperExtensions).GetMethod(nameof(_addTypeHandlerImpl),
                BindingFlags.Static | BindingFlags.NonPublic) ??
            throw new InvalidOperationException("failed to find _addTypeHandlerMethod method");

        private static void _addTypeHandler(Type valueOfType, Type ut)
        {
            var gm = _addTypeHandlerMethod.MakeGenericMethod(ut, valueOfType);
            gm.Invoke(null, Array.Empty<object>());
        }

        private static void _configureValueOfDapperTypeHandlers(IEnumerable<Type> types)
        {
            foreach (var valueOfType in types)
            {
                if (valueOfType.IsValueOfType(out var ut))
                {
                    _addTypeHandler(valueOfType, ut);
                }
            }
        }

        public static void ConfigureValueOfDapperTypeHandlers(params Type[] types)
        {
            _configureValueOfDapperTypeHandlers(types);
        }

        public static void ConfigureValueOfDapperTypeHandlers(params Assembly[] assemblies)
        {
            _configureValueOfDapperTypeHandlers(assemblies.SelectMany(a => a.DefinedTypes).Select(t => t.AsType()));
        }
    }
}