using System.Data.SQLite;
using Dapper;
using ValueOf.Extensions.Dapper;
using ValueOf.Extensions.Tests.Models;

namespace ValueOf.Extensions.Tests;

public class ValueOfDapperTypeHandlerTests
{
    static ValueOfDapperTypeHandlerTests()
    {
        ValueOfDapperExtensions.ConfigureValueOfDapperTypeHandlers(typeof(TestUserId), typeof(TestEmail));
    }

    private record TestModel
    {
        public required TestUserId Id { get; set; }
        public required TestEmail Email { get; set; }
    }


    [Fact]
    public void ValueOfType_Passes_IsValueOfTypeTest()
    {
        string constr = "Data Source=:memory:";

        using var con = new SQLiteConnection(constr);
        con.Open();

        var actual = con.QueryFirst<TestModel>("select 12 as Id, 'imyuanjian@gmail.com' as Email");
        var expected = new TestModel
        {
            Id = TestUserId.From(12),
            Email = TestEmail.From("imyuanjian@gmail.com"),
        };

        // Assert.Equal(expected, actual);
    }
}