using System;
using System.ComponentModel;
using System.Globalization;

namespace ValueOf.Extensions
{
    public class ValueOfTypeConverter<TU, T> : TypeConverter where T : ValueOf<TU, T>, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(TU))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var debug = $"{destinationType.Name} == {typeof(TU).Name} when {typeof(T).Name}";
            Console.WriteLine(debug);
            if (destinationType == typeof(TU))
            {
                return true;
            }

            if (typeof(TU) == typeof(int) && destinationType == typeof(string)) return false;
            // return false;
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is TU tValue)
            {
                return ValueOf<TU, T>.From(tValue);
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

            var typeofTValue = typeof(TU);
            var typeofTThis = typeof(T);

            var val = (T)value;
            var underlyingVal = val.Value;

            Console.WriteLine($"{typeofTValue.Name} vs {typeofTThis.Name}: {underlyingVal}");

            var converted = base.ConvertTo(context, culture, underlyingVal, destinationType);
            return converted;
        }
    }
}