using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueOf.Extensions;
using ValueOf.Extensions.Dapper;
using ValueOf.Extensions.Examples;
using ValueOf.Extensions.Examples.Database;
using ValueOf.Extensions.Examples.Models;
using ValueOf.Extensions.SwashbuckleSwagger;

var builder = WebApplication.CreateBuilder(args);


ValueOfTypeExtensions.ConfigureValueOfTypeConverters(typeof(UserId).Assembly);
ValueOfDapperExtensions.ConfigureValueOfDapperTypeHandlers(typeof(UserId).Assembly);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new ValueOfJsonAdapterFactory());
#if DEBUG
    options.JsonSerializerOptions.WriteIndented = true;
#endif
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.IgnoreObsoleteActions();
    opts.IgnoreObsoleteProperties();
    opts.UseAllOfForInheritance();
    opts.MapValueOfTypesInAssemblies(null, typeof(EmailAddress).Assembly);
});

builder.Services.AddDbContext<DemoDbContext>(opts => { opts.UseSqlite("Data Source=demo.db"); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/searchUsers",
    async ([FromServices] DemoDbContext _dbContext, [FromQuery] UserId? userId, [FromQuery] EmailAddress? email) =>
    {
        IQueryable<User> q = _dbContext.Users;
        if (userId != null)
        {
            q = q.Where(u => u.Id == userId);
        }

        if (email != null)
        {
            q = q.Where(u => u.Email == email);
        }

        return q.ToList();
    });


app.Run();

public partial class Program
{
}