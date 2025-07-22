using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.Abstractions;

namespace ValueOf.Extensions.Tests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper output;

    private readonly CustomWebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        this.output = output;
    }

    [Fact]
    public async Task ValueOf_FromRoute_Works()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("Users/12");

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        var content = await response.Content.ReadAsStringAsync();
        var id = JsonDocument.Parse(content).RootElement.GetProperty("id").GetInt32();
        var email = JsonDocument.Parse(content).RootElement.GetProperty("email").GetString();
        Assert.Equal(12, id);
        Assert.Equal("imyuanjian@gmail.com", email);
    }

    [Fact]
    public async Task ValueOf_FromQuery_Works()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("Users/findByEmail?email=xiaobao@gmail.com");

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        var content = await response.Content.ReadAsStringAsync();
        var id = JsonDocument.Parse(content).RootElement.GetProperty("id").GetInt32();
        var email = JsonDocument.Parse(content).RootElement.GetProperty("email").GetString();
        Assert.Equal(22, id);
        Assert.Equal("xiaobao@gmail.com", email);
    }

    [Fact]
    public async Task ValueOf_MinimalAPI_Works()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/searchUsers?userId=22");

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        var content = await response.Content.ReadAsStringAsync();
        var first = JsonDocument.Parse(content).RootElement.EnumerateArray().FirstOrDefault();
        var id = first.GetProperty("id").GetInt32();
        var email = first.GetProperty("email").GetString();
        Assert.Equal(22, id);
        Assert.Equal("xiaobao@gmail.com", email);
    }

    [Fact]
    public async Task SwaggerWorks()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        var reader = new OpenApiStreamReader();
        using var s = await response.Content.ReadAsStreamAsync();
        var doc = reader.Read(s, out var diagnostic);
        if (diagnostic.Errors.Count > 0)
        {
            foreach (var error in diagnostic.Errors)
            {
                Assert.Fail($"Parse swagger error: {error.Message}");
            }
        }

        Assert.NotNull(doc);
        var byId = doc.Paths["/Users/{id}"];
        var byIdOp = byId.Operations.Single(o => o.Key == OperationType.Get).Value;
        var idParam = byIdOp.Parameters.Single();
        Assert.Equal("id", idParam.Name);
        Assert.Equal(ParameterLocation.Path, idParam.In);
        Assert.Equal("integer", idParam.Schema.Type);
        Assert.Equal("int32", idParam.Schema.Format);
        var idParamExts = idParam.Schema.Extensions.TryGetValue("x-valueof-type", out var val)
            ? (OpenApiObject)val
            : throw new InvalidOperationException("x-valueof-type missing");
        Assert.Equal("UserId", ((OpenApiString)idParamExts["name"]).Value);
    }
}