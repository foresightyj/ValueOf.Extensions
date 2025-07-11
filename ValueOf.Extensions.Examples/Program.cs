using System.Text.Json;
using System.Text.Json.Serialization;
using ValueOf.Extensions;
using ValueOf.Extensions.Examples;
using ValueOf.Extensions.Examples.Models;
using ValueOf.Extensions.SwashbuckleSwagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new ValueOfTypeConverterJsonAdapterFactory());
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
    opts.MapValueOfTypesInAssemblies(typeof(EmailAddress).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();