using System;
using System.ComponentModel;
using System.Globalization;

namespace ValueOf.Extensions
{
    public class ValueOfTypeConverter<TValue, TThis> : TypeConverter where TThis : ValueOf<TValue, TThis>, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(TValue))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var debug = $"{destinationType.Name} == {typeof(TValue).Name} when {typeof(TThis).Name}";
            Console.WriteLine(debug);
            if (destinationType == typeof(TValue))
            {
                return true;
            }

            if (typeof(TValue) == typeof(int) && destinationType == typeof(string)) return false;
            // return false;
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is TValue tValue)
            {
                return ValueOf<TValue, TThis>.From(tValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value,
            Type destinationType)
        {
            if (value is null) return null;


            if (destinationType == typeof(TValue))
            {
                return ((TThis)value).Value;
            }

            var typeofTValue = typeof(TValue);
            var typeofTThis = typeof(TThis);

            var val = (TThis)value;
            var underlyingVal = val.Value;

            Console.WriteLine($"{typeofTValue.Name} vs {typeofTThis.Name}: {underlyingVal}");

            var converted = base.ConvertTo(context, culture, underlyingVal, destinationType);
            return converted;
        }
    }
}