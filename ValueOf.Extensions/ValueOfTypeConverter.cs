﻿using System.ComponentModel;
using System.Globalization;

namespace ValueOf.Extensions;

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

        return base.ConvertTo(context, culture, value, destinationType);
    }
}