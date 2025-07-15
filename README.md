# ValueOf.Extensions

This library contains a set of adapters/utilities you might need when using the
popular [ValueOf](https://github.com/mcintyre321/ValueOf) library. It helps you in these areas:

1. JSON serialization
2. Dapper/EF.Core
3. Swagger/OpenAPI

## Feature 1: JSON serialization adapter for System.Text.Json & Newtonsoft.Json

For System.Text.JSON, you will need an adapter (see [the reason]( https://github.com/dotnet/runtime/issues/38812 ))

```csharp
    var options = new JsonSerializerOptions();
    options.Converters.Add(new TypeConverterJsonAdapterFactory());
    System.Text.Json.JsonSerializer.Serialize(_dut, options);
```

For Newtonsoft.Json, it is automatic.

see [Document a System.Text.Json TypeConverter to JsonConverter Adapter · Issue #1761 · dotnet/runtime]( https://github.com/dotnet/runtime/issues/1761 )

and [System.Text.Json does not support TypeConverters · Issue #38812 · dotnet/runtime]( https://github.com/dotnet/runtime/issues/38812 )

