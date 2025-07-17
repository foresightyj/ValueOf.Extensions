using System.Diagnostics.CodeAnalysis;

namespace ValueOf.Extensions.Examples;

public interface IValueOfParsable<TU, T> : IParsable<T> where T : ValueOf<TU, T>, IParsable<T>, new()
{
    public static T Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
    {
        throw new NotImplementedException();
    }
}

public sealed class UserId : ValueOf<int, UserId>, IValueOfParsable<int, UserId>
{
    protected override void Validate()
    {
        var userId = Value;
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId) + $" must be positive integer but is: {userId}");
    }

}