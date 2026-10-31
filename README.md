# liveapi-dotnet

.NET client for the [LiveAPI](https://liveapi.io) reference-data API.

The token spends calls on your account. Keep it in an environment variable on your server. Do not put it in a browser page or a mobile app.

```csharp
var client = new LiveApi.Client(Environment.GetEnvironmentVariable("LIVEAPI_TOKEN")!);
var phone = await client.ValidatePhoneAsync("0555 111 22 33", "TR");
```

Reference this project, or add the package once it is published to NuGet.

Docs: https://liveapi.io/docs

IP lookups include `Attribution` and `AttributionUrl`. Keep those fields when you store or show a DB-IP result. A validation result with `Valid` false is still HTTP 200 and uses one call. Only a non-2xx response is free.
