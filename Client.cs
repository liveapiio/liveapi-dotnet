using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiveApi;

/// <summary>
/// Calls the LiveAPI reference-data endpoints.
/// A token belongs on a server. Do not ship it inside a browser or mobile app.
/// </summary>
public sealed class Client : IDisposable
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient http;
    private readonly bool ownsClient;
    private readonly string token;
    private readonly string baseUrl;

    public Client(string token, string baseUrl = "https://liveapi.io", HttpClient? http = null)
    {
        this.token = token;
        this.baseUrl = baseUrl.TrimEnd('/');
        ownsClient = http is null;
        this.http = http ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    }

    public Task<IpLocation> CallerIpAsync(CancellationToken cancellationToken = default) => GetAsync<IpLocation>("/v1/ip", cancellationToken);

    public Task<IpLocation> LookupIpAsync(string ip, CancellationToken cancellationToken = default) =>
        GetAsync<IpLocation>("/v1/ip/" + Uri.EscapeDataString(ip), cancellationToken);

    public Task<string> CallerFieldAsync(string field, CancellationToken cancellationToken = default) =>
        TextAsync("/v1/ip/" + Uri.EscapeDataString(field), cancellationToken);

    public Task<string> LookupFieldAsync(string ip, string field, CancellationToken cancellationToken = default) =>
        TextAsync("/v1/ip/" + Uri.EscapeDataString(ip) + "/" + Uri.EscapeDataString(field), cancellationToken);

    public Task<List<Country>> CountriesAsync(string? query = null, CancellationToken cancellationToken = default) =>
        GetAsync<List<Country>>("/v1/countries" + Query(("q", query)), cancellationToken);

    public Task<Country> CountryAsync(string code, CancellationToken cancellationToken = default) =>
        GetAsync<Country>("/v1/countries/" + Uri.EscapeDataString(code), cancellationToken);

    public Task<Country> CountryWithGeoShapeAsync(string code, CancellationToken cancellationToken = default) =>
        GetAsync<Country>("/v1/countries/" + Uri.EscapeDataString(code) + "?include=geoshape", cancellationToken);

    public Task<List<Division>> DivisionsAsync(string country, CancellationToken cancellationToken = default) =>
        GetAsync<List<Division>>("/v1/countries/" + Uri.EscapeDataString(country) + "/divisions", cancellationToken);

    public Task<Division> DivisionByCodeAsync(string country, string code, CancellationToken cancellationToken = default) =>
        GetAsync<Division>("/v1/countries/" + Uri.EscapeDataString(country) + "/divisions/" + Uri.EscapeDataString(code), cancellationToken);

    public Task<List<Division>> DivisionChildrenByCodeAsync(string country, string code, CancellationToken cancellationToken = default) =>
        GetAsync<List<Division>>("/v1/countries/" + Uri.EscapeDataString(country) + "/divisions/" + Uri.EscapeDataString(code) + "/children", cancellationToken);

    public Task<Division> DivisionAsync(string id, CancellationToken cancellationToken = default) =>
        GetAsync<Division>("/v1/divisions/" + Uri.EscapeDataString(id), cancellationToken);

    public Task<List<Division>> DivisionChildrenAsync(string id, CancellationToken cancellationToken = default) =>
        GetAsync<List<Division>>("/v1/divisions/" + Uri.EscapeDataString(id) + "/children", cancellationToken);

    public Task<List<Currency>> CurrenciesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<Currency>>("/v1/currencies", cancellationToken);

    public Task<List<CallingCode>> CallingCodesAsync(string? country = null, CancellationToken cancellationToken = default) =>
        GetAsync<List<CallingCode>>("/v1/phones/codes" + Query(("country", country)), cancellationToken);

    public Task<PhoneValidation> ValidatePhoneAsync(string number, string? country = null, CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, string> { ["number"] = number };
        if (!string.IsNullOrEmpty(country)) body["country"] = country;
        return PostAsync<PhoneValidation>("/v1/phones/validate", body, cancellationToken);
    }

    public Task<List<PlateFormat>> PlateFormatsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<List<PlateFormat>>("/v1/plates/formats", cancellationToken);

    public Task<PlateValidation> ValidatePlateAsync(string plate, string country, CancellationToken cancellationToken = default) =>
        PostAsync<PlateValidation>("/v1/plates/validate", new { plate, country }, cancellationToken);

    public async Task<Rates> RatesAsync(string? source = null, string? date = null, CancellationToken cancellationToken = default)
    {
        var wire = await GetAsync<RatesWire>("/v1/rates" + Query(("source", source), ("date", date)), cancellationToken);
        return Rates.FromWire(wire, Json);
    }

    public Task<Conversion> ConvertAsync(string from, string to, double? amount = null, string? source = null, string? date = null, string? tcmbRate = null, CancellationToken cancellationToken = default) =>
        GetAsync<Conversion>("/v1/rates/convert" + Query(("from", from), ("to", to), ("amount", amount?.ToString(System.Globalization.CultureInfo.InvariantCulture)), ("source", source), ("date", date), ("tcmb_rate", tcmbRate)), cancellationToken);

    public Task<Distance> DistanceAsync(Point from, Point to, CancellationToken cancellationToken = default) =>
        PostAsync<Distance>("/v1/geo/distance", new { from, to }, cancellationToken);

    public Task<IbanValidation> ValidateIbanAsync(string iban, CancellationToken cancellationToken = default) =>
        PostAsync<IbanValidation>("/v1/iban/validate", new { iban }, cancellationToken);

    public Task<TrNumberValidation> ValidateIdentityAsync(string number, CancellationToken cancellationToken = default) =>
        PostAsync<TrNumberValidation>("/v1/tr/identity/validate", new { number }, cancellationToken);

    public Task<TrNumberValidation> ValidateTaxNumberAsync(string number, CancellationToken cancellationToken = default) =>
        PostAsync<TrNumberValidation>("/v1/tr/tax-number/validate", new { number }, cancellationToken);

    public Task<HolidayList> HolidaysAsync(string country, int? year = null, CancellationToken cancellationToken = default) =>
        GetAsync<HolidayList>("/v1/holidays" + Query(("country", country), ("year", year?.ToString(System.Globalization.CultureInfo.InvariantCulture))), cancellationToken);

    public Task<BusinessDays> BusinessDaysAsync(object request, CancellationToken cancellationToken = default) =>
        PostAsync<BusinessDays>("/v1/business-days", request, cancellationToken);

    public Task<Timezone> TimezoneAsync(double? latitude = null, double? longitude = null, string? country = null, CancellationToken cancellationToken = default) =>
        GetAsync<Timezone>("/v1/timezone" + Query(
            ("lat", latitude?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("lon", longitude?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("country", country)), cancellationToken);

    public Task<EmailCheck> ValidateEmailAsync(string email, bool? dns = null, CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object> { ["email"] = email };
        if (dns is not null) body["dns"] = dns.Value;
        return PostAsync<EmailCheck>("/v1/email/validate", body, cancellationToken);
    }

    public Task<VatCheck> ValidateVatAsync(string vatNumber, string? country = null, CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, string> { ["vat_number"] = vatNumber };
        if (!string.IsNullOrEmpty(country)) body["country"] = country;
        return PostAsync<VatCheck>("/v1/vat/validate", body, cancellationToken);
    }

    /// <summary>Lists the national ID, tax and company number checks.</summary>
    public async Task<List<IdValidator>> IdValidatorsAsync(CancellationToken cancellationToken = default) =>
        (await GetAsync<IdValidatorList>("/v1/validate", cancellationToken).ConfigureAwait(false)).Data;

    /// <summary>Checks a national number offline, for example ("ES", "nif", "12345678Z").</summary>
    public Task<IdValidation> ValidateIdAsync(string country, string type, string value, CancellationToken cancellationToken = default) =>
        PostAsync<IdValidation>($"/v1/validate/{Uri.EscapeDataString(country)}/{Uri.EscapeDataString(type)}", new Dictionary<string, string> { ["value"] = value }, cancellationToken);

    public Task<PostalCodeCountries> PostalCodeCountriesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<PostalCodeCountries>("/v1/postal-codes", cancellationToken);

    public Task<PostalCodeLookup> PostalCodeAsync(string country, string code, CancellationToken cancellationToken = default) =>
        GetAsync<PostalCodeLookup>($"/v1/postal-codes/{Uri.EscapeDataString(country)}/{Uri.EscapeDataString(code)}", cancellationToken);

    /// <summary>Country and administrative area for a point.</summary>
    public Task<ReverseGeocode> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default) =>
        GetAsync<ReverseGeocode>("/v1/reverse" + Query(
            ("lat", latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("lon", longitude.ToString(System.Globalization.CultureInfo.InvariantCulture))), cancellationToken);

    /// <summary>Parses a User-Agent string, for example one your server received.</summary>
    public Task<UserAgent> ParseUserAgentAsync(string ua, CancellationToken cancellationToken = default) =>
        PostAsync<UserAgent>("/v1/user-agent", new Dictionary<string, string> { ["ua"] = ua }, cancellationToken);

    /// <summary>Checks an EAN-8, UPC-A, EAN-13, GTIN-14 or ISBN check digit.</summary>
    public Task<BarcodeValidation> ValidateBarcodeAsync(string code, CancellationToken cancellationToken = default) =>
        GetAsync<BarcodeValidation>("/v1/barcodes/validate" + Query(("code", code)), cancellationToken);

    public void Dispose()
    {
        if (ownsClient) http.Dispose();
    }

    private Task<T> GetAsync<T>(string path, CancellationToken cancellationToken) => SendAsync<T>(HttpMethod.Get, path, null, cancellationToken);

    private Task<T> PostAsync<T>(string path, object body, CancellationToken cancellationToken) => SendAsync<T>(HttpMethod.Post, path, body, cancellationToken);

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(method, path, body, "application/json", cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure((int)response.StatusCode, raw);
        var value = JsonSerializer.Deserialize<T>(raw, Json);
        if (value is null) throw new LiveApiException((int)response.StatusCode, "invalid_response", "LiveAPI returned a response that is not JSON.");
        return value;
    }

    private async Task<string> TextAsync(string path, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(HttpMethod.Get, path, null, "text/plain", cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure((int)response.StatusCode, raw);
        return raw;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body, string accept, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new LiveApiException(401, "unauthorized", "Bearer token is missing.");
        using var request = new HttpRequestMessage(method, baseUrl + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
        request.Headers.UserAgent.ParseAdd("liveapi-dotnet/0.1.0");
        if (body is not null) request.Content = JsonContent.Create(body, options: Json);
        return await http.SendAsync(request, cancellationToken);
    }

    private static LiveApiException Failure(int status, string raw)
    {
        try
        {
            using var document = JsonDocument.Parse(raw);
            if (document.RootElement.TryGetProperty("error", out var error) && error.TryGetProperty("message", out var message))
            {
                var code = error.TryGetProperty("code", out var codeElement) ? codeElement.GetString() ?? "" : "";
                return new LiveApiException(status, code, message.GetString() ?? "LiveAPI request failed.");
            }
        }
        catch (JsonException)
        {
        }
        var text = raw.Trim();
        return new LiveApiException(status, "", text.Length == 0 ? "LiveAPI request failed." : text);
    }

    private static string Query(params (string Key, string? Value)[] pairs)
    {
        var text = string.Join("&", pairs.Where(pair => !string.IsNullOrEmpty(pair.Value)).Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value!)}"));
        return text.Length == 0 ? "" : "?" + text;
    }
}
