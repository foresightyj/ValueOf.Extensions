using System.Text.Json;
using Newtonsoft.Json;
using ValueOf.Extensions.NewtonsoftJson;
using ValueOf.Extensions.Tests.Models;

namespace ValueOf.Extensions.Tests;

public class ValueOfJsonConverterTests
{
    private readonly TestModel _dut = new()
    {
        UserId = TestUserId.From(12),
        PostId = TestPostId.From(345),
        Email = TestEmail.From("imyuanjian@gmail.com"),
    };

    [Fact]
    void ValueOf_WorksWith_SystemTextJson()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new ValueOfJsonAdapterFactory() },
        };

        var correctJson = System.Text.Json.JsonSerializer.Serialize(_dut, options);
        Assert.Equal("""{"UserId":12,"PostId":345,"Email":"imyuanjian@gmail.com"}""", correctJson);
        var d = System.Text.Json.JsonSerializer.Deserialize<TestModel>(correctJson, options)!;
        Assert.Equal(d.Email, _dut.Email);
    }

    [Fact]
    void ValueOf_WorksWith_NewtonsoftJson()
    {
        var settings = new JsonSerializerSettings
        {
            Converters = [new ValueOfNewtonsoftConverter()],
        };
        Assert.Equal("12", JsonConvert.SerializeObject(_dut.UserId, settings));
        Assert.Equal("\"imyuanjian@gmail.com\"", JsonConvert.SerializeObject(_dut.Email, settings));
        var json = JsonConvert.SerializeObject(_dut, settings);
        const string expected = """{"UserId":12,"PostId":345,"Email":"imyuanjian@gmail.com"}""";
        Assert.Equal(expected, json);
        var d = JsonConvert.DeserializeObject<TestModel>(json, settings)!;
        Assert.Equal(_dut.Email, d.Email);
    }
}