# liveapi-dotnet

.NET client for the [LiveAPI](https://liveapi.io) reference-data API.

The token spends calls on your account. Keep it in an environment variable on your server. Do not put it in a browser page or a mobile app.

```csharp
var client = new LiveApi.Client(Environment.GetEnvironmentVariable("LIVEAPI_TOKEN")!);
var phone = await client.ValidatePhoneAsync("0555 111 22 33", "TR");
```

Reference this project, or add the package once it is published to NuGet.

New in this version: `IdValidatorsAsync`, `ValidateIdAsync`, `PostalCodeCountriesAsync`, `PostalCodeAsync`, `ReverseGeocodeAsync`, `ParseUserAgentAsync`, `ValidateBarcodeAsync`, and the IP risk fields `IsTor`, `IsHosting`, `HostingProvider`.

```csharp
var id = await client.ValidateIdAsync("ES", "nif", "12345678Z");
var barcode = await client.ValidateBarcodeAsync("4006381333931");
var postal = await client.PostalCodeAsync("DE", "10115");
var area = await client.ReverseGeocodeAsync(41.01, 28.97);
var ua = await client.ParseUserAgentAsync(Request.Headers.UserAgent.ToString());
```

Docs: https://liveapi.io/docs

IP lookups include `Attribution` and `AttributionUrl`. Keep those fields when you store or show a DB-IP result. Postal-code results carry a GeoNames `attribution` (CC BY 4.0); keep it too. A validation result with `Valid` false is still HTTP 200 and uses one call. Only a non-2xx response is free.
