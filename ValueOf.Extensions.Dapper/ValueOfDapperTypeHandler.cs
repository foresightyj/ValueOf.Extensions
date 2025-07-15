using System;
using System.Data;
using Dapper;

namespace ValueOf.Extensions.Dapper
{
    public class ValueOfDapperTypeHandler<TU, T> : SqlMapper.TypeHandler<T> where T : ValueOf<TU, T>, new()
    {
        public override void SetValue(IDbDataParameter parameter, T time)
        {
            parameter.Value = time.Value?.ToString();
        }

        public override T Parse(object value)
        {
            if (value == null) return default;

            if (!(value is TU))
            {
                value = Convert.ChangeType(value, typeof(TU));
            }

            return ValueOf<TU, T>.From((TU)value);
        }
    }
}