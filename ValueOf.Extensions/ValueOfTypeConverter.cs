using System;
using System.ComponentModel;
using System.Globalization;

namespace ValueOf.Extensions
{
    public class ValueOfTypeConverter<TU, T> : TypeConverter where T : ValueOf<TU, T>, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(TU) || sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(TU))
            {
                return true;
            }

            if (typeof(TU) == typeof(int) && destinationType == typeof(string)) return false;
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is TU tValue)
            {
                return ValueOf<TU, T>.From(tValue);
            }

            if (value is string s)
            {
                var parsed = BuiltinTypeParser.Parse<TU>(s);
                return ValueOf<TU, T>.From(parsed);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value,
            Type destinationType)
        {
            if (value is null) return null;

            if (destinationType == typeof(TU))
            {
                return ((T)value).Value;
            }

            var val = (T)value;
            var underlyingVal = val.Value;

            var converted = base.ConvertTo(context, culture, underlyingVal, destinationType);
            return converted;
        }
    }
}