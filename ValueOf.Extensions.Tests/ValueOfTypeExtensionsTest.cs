using ValueOf.Extensions.Tests.Models;

namespace ValueOf.Extensions.Tests;

public class ValueOfTypeExtensionsTest
{
    [Fact]
    public void ValueOfType_Passes_IsValueOfTypeTest()
    {
        Assert.True(typeof(TestEmail).IsValueOfType(out var underlyingType));
        Assert.Equal(typeof(string), underlyingType);
    }

    [Fact]
    public void AnyOtherType_Fails_IsValueOfTypeTest()
    {
        Assert.False(typeof(int).IsValueOfType(out _));
        Assert.False(typeof(List<int>).IsValueOfType(out _));
        Assert.False(typeof(Dictionary<int, string>).IsValueOfType(out _));
    }
}