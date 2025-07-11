using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ValueOf.Extensions.Tests;

[TypeConverter(typeof(ValueOfTypeConverter<string, ProductCode>))]
public class ProductCode : ValueOf<string, ProductCode>
{
    private static readonly Regex ValidPatt = new(@"^[A-Z][A-Z0-9_]*$");

    protected override void Validate()
    {
        var productCode = Value;
        if (string.IsNullOrEmpty(productCode)) throw new ArgumentNullException(nameof(productCode));
        if (!ValidPatt.IsMatch(productCode))
            throw new ArgumentException(nameof(productCode) + $" has invalid format, value: {Value}");
    }
}

public class TestProductCode
{
    public required ProductCode Code { get; set; }
}

public class ValueOfTypeConverterTests
{
    private readonly TestProductCode _dut = new()
    {
        Code = ProductCode.From("IPAD_12"),
    };

    [Fact]
    void SystemTextJsonSerialization_WorksWith_TypeConverterJsonAdapterFactory()
    {
        var incorrectJson = System.Text.Json.JsonSerializer.Serialize(_dut);
        Assert.Equal("""{"Code":{"Value":"IPAD_12"}}""", incorrectJson);

        var options = new JsonSerializerOptions();
        options.Converters.Add(new TypeConverterJsonAdapterFactory());

        var correctJson = System.Text.Json.JsonSerializer.Serialize(_dut, options);
        Assert.Equal("""{"Code":"IPAD_12"}""", correctJson);
        var d = System.Text.Json.JsonSerializer.Deserialize<TestProductCode>(correctJson, options)!;
        Assert.Equal(d.Code, _dut.Code);
    }

    [Fact]
    void WorksWithNewtonsoftJsonAutomatically()
    {
        Assert.Equal("\"IPAD_12\"", JsonConvert.SerializeObject(_dut.Code));
        Assert.Equal("""{"Code":"IPAD_12"}""", JsonConvert.SerializeObject(_dut));
    }
}