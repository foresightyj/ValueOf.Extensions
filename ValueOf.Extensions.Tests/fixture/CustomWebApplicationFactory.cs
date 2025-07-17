using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValueOf.Extensions.Examples.Database;

namespace ValueOf.Extensions.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(x =>
        {
            x.Remove(x.Single(d => d.ServiceType == typeof(DbContextOptions<DemoDbContext>)));

            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open(); // MUST keep open for lifetime of tests

            x.AddDbContext<DemoDbContext>(options => { options.UseSqlite(connection); });

            SeedTestData(x);
        });

        builder.UseEnvironment("Development");
    }

    private void SeedTestData(IServiceCollection x)
    {
        var sp = x.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory<Program>>>();

        try
        {
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the test DB.");
            throw;
        }
    }
}