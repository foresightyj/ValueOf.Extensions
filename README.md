ValueOf.Extensions
==================

This library contains a set of adapters/utilities you might need when using the popular [ValueOf](https://github.com/mcintyre321/ValueOf) library. 

Below is a list of features supported in each package:

# Package: ValueOf.Extensions

## System.Text.Json JSON serialization/deserialization with `ValueOfJsonAdapterFactory`

```cs
var options = new JsonSerializerOptions
{
    Converters = { new ValueOfJsonAdapterFactory() },
};
```

or in ASP.NET Core:

```cs
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ValueOfJsonAdapterFactory());
});
```

## TypeConverter with `ValueOfTypeConverter<TU, T>`

This is needed if you want to be able to parse a `ValueOf` type from strings. Typically you'll need it when you want to bind API parameters e.g.  `[FromRoute] UserId id` & `[FromQuery] EmailAddress email` in ASP.NET Core.

You can use this provided extension method to add all `ValueOf` types collectively.

```cs
    ValueOfTypeExtensions.ConfigureValueOfTypeConverters(typeof(UserId).Assembly);
```

# Package: ValueOf.Extensions.NewtonsoftJson

If you are using Newtonsoft.JSON, this package fixes serialization/deserialization with a custom JsonConverter `ValueOfNewtonsoftConverter`.

```cs
var settings = new JsonSerializerSettings
{
    Converters = [new ValueOfNewtonsoftConverter()],
};
```

# Package: ValueOf.Extensions.Dapper

With this package, you can have your column be of `ValueOf` types. 

You can use the provided extension method to register all `ValueOfDapperTypeHandler`s from assemblies with ease:

```cs
ValueOfDapperExtensions.ConfigureValueOfDapperTypeHandlers(typeof(UserId).Assembly);
```

# Package: ValueOf.Extensions.EFCore

This package adds `ValueConverter` automatically for your `ValueOf` types if you use them in your EF.Core entity columns. The usage is simple:

```cs
    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        base.ConfigureConventions(b);
        b.ConfigureValueOfConversions(typeof(UserId).Assembly);
    }
```


# Package: ValueOf.Extensions.Swagger

If you `ValueOf`, swagger will still consider a `ValueOf` type a complex type instead of a simple type. This package helps you fix that by picking the most suitable `type` & `format` based on the underlying type.

This extension method helps you map all your `ValueOf` types to the correct schema.
```cs
    opts.MapValueOfTypesInAssemblies(null, typeof(EmailAddress).Assembly);
```

If `type` & `format` picked for you doesn't suit your needs, you can use the `SwaggerTypeMap? typeMapOverride` parameter in `MapValueOfTypesInAssemblies` to customize.

NOTE: currently I only handled `Swashbuckle.AspNetCore`. Let me know or send me PR if you want integration for other Swagger/Open API solutions.

# Package: ValueOf.Extensions.ParsableGenerator

There are many ways to bind route or query parameters to your custom types in minimal APIs but unfortunately `TypeConverter` is not one of them. A non-generator solution would have worked if [this issue](https://github.com/dotnet/aspnetcore/issues/58136) is fixed. Anyway, this source code generator will generate methods implementing `IParsable<T>` for you automatically.

You just need to include this nuget package & mark your ValueOf subclass as `partial`. 

Additionally configure for System.Text.JSON as follows:

```
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new ValueOfJsonAdapterFactory()));
```

Note that System.Text.JSON configuration for Minimal APIs is different from that for the conventional controllers.

# Examples

Do check out the `ValueOf.Extensions.Examples` directory as it contains an comprehensive ASP.NET Core project demonstrating capabilities of all nuget packages in this project.

# References

see [Document a System.Text.Json TypeConverter to JsonConverter Adapter · Issue #1761 · dotnet/runtime]( https://github.com/dotnet/runtime/issues/1761 )

and [System.Text.Json does not support TypeConverters · Issue #38812 · dotnet/runtime]( https://github.com/dotnet/runtime/issues/38812 )

