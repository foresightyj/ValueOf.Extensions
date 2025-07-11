using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ValueOf.Extensions.NewtonsoftJson;

namespace ValueOf.Extensions.Tests
{
    public class TestUser
    {
        public required TestUserId Id { get; set; }
        public required TestEmail Email { get; set; }
    }

    public class ValueOfTypeConverterTests
    {
        private readonly TestUser _dut = new()
        {
            Id = TestUserId.From(12),
            Email = TestEmail.From("imyuanjian@gmail.com"),
        };

        [Fact]
        void SystemTextJsonSerialization_WorksWith_TypeConverterJsonAdapterFactory()
        {
            var incorrectJson = System.Text.Json.JsonSerializer.Serialize(_dut);
            Assert.Equal("""{"Id":{"Value":12},"Email":{"Value":"imyuanjian@gmail.com"}}""", incorrectJson);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ValueOfTypeConverterJsonAdapterFactory());

            var correctJson = System.Text.Json.JsonSerializer.Serialize(_dut, options);
            Assert.Equal("""{"Id":12,"Email":"imyuanjian@gmail.com"}""", correctJson);
            var d = System.Text.Json.JsonSerializer.Deserialize<TestUser>(correctJson, options)!;
            Assert.Equal(d.Email, _dut.Email);
        }

        [Fact]
        void SystemTextJsonSerialization_WorksWith_ValueOfNewtonsoftConverter()
        {
            var settings = new JsonSerializerSettings()
            {
                Converters = [new ValueOfNewtonsoftConverter()],
            };
            Assert.Equal("12", JsonConvert.SerializeObject(_dut.Id, settings));
            Assert.Equal("\"imyuanjian@gmail.com\"", JsonConvert.SerializeObject(_dut.Email, settings));
            var json = JsonConvert.SerializeObject(_dut, settings);
            const string expected = """{"Id":12,"Email":"imyuanjian@gmail.com"}""";
            Assert.Equal(expected, json);
            var d = JsonConvert.DeserializeObject<TestUser>(json, settings)!;
            Assert.Equal(_dut.Email, d.Email);
        }
    }
}