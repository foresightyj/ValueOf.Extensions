using System.ComponentModel;

namespace ValueOf.Extensions.Tests;

[TypeConverter(typeof(ValueOfTypeConverter<int, TestUserId>))]
public sealed class TestUserId : ValueOf<int, TestUserId>
{
    protected override void Validate()
    {
        var userId = Value;
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId) + $" must be positive integer but is: {userId}");
    }
}