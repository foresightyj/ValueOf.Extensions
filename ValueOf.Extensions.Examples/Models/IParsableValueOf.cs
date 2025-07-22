using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ValueOf.Extensions.Examples;

public interface IParsableValueOf<TU, T> : IParsable<T> where T : ValueOf<TU, T>, IParsable<T>, new()
{
    static T IParsable<T>.Parse(string s, IFormatProvider? provider)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFrom(s)!;
    }

    static bool IParsable<T>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out T result)
    {
        result = default(T);
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter.CanConvertFrom(typeof(string)))
        {
            result = (T)converter.ConvertFrom(s!)!;
            return true;
        }

        return false;
    }
}