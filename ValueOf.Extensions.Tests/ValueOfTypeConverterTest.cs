using System.ComponentModel;

namespace ValueOf.Extensions.Tests;

public class ValueOfTypeConverterTest
{
    [TypeConverter(typeof(ValueOfTypeConverter<int, TestInt>))]
    private sealed class TestInt : ValueOf<int, TestInt>
    {
        protected override void Validate()
        {
            var testInt = Value;
            if (testInt <= 0)
                throw new ArgumentOutOfRangeException(nameof(testInt) + $" must be positive integer but is: {testInt}");
        }
    }

    [Fact]
    public void TypeConverterWorks()
    {
        var converter = TypeDescriptor.GetConverter(typeof(TestInt));
        Assert.NotNull(converter);
        Assert.True(converter.CanConvertTo(null, typeof(int)));
        Assert.False(converter.CanConvertTo(null, typeof(long)));
        Assert.False(converter.CanConvertTo(null, typeof(string)));

        var expected = TestInt.From(12);
        var actual = (TestInt)converter.ConvertFrom(12)!;
        Assert.Equal(expected, actual);
        Assert.Equal(12, (int)converter.ConvertTo(expected, typeof(int))!);
    }

    [Fact]
    public void TypeConverterWorks_For_String()
    {
        var converter = TypeDescriptor.GetConverter(typeof(TestInt));
        Assert.NotNull(converter);
        var expected = TestInt.From(12);
        var actual = (TestInt)converter.ConvertFrom("12")!;
        Assert.Equal(expected, actual);
    }
}