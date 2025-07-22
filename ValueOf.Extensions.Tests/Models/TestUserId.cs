namespace ValueOf.Extensions.Tests.Models;

public sealed partial class TestUserId : ValueOf<int, TestUserId>
{
    protected override void Validate()
    {
        var userId = Value;
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId) + $" must be positive integer but is: {userId}");
    }
}