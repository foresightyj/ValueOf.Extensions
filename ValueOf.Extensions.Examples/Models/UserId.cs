using System.Diagnostics.CodeAnalysis;

namespace ValueOf.Extensions.Examples;

public sealed partial class UserId : ValueOf<int, UserId>
{
    protected override void Validate()
    {
        var userId = Value;
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId) + $" must be positive integer but is: {userId}");
    }
}