using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiveApi;

public sealed class Currency
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Symbol { get; set; }
}

public sealed class Language
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public sealed class Country
{
    public string Code { get; set; } = "";
    public string Iso3 { get; set; } = "";
    public string? Numeric { get; set; }
    public string Name { get; set; } = "";
    public string OfficialName { get; set; } = "";
    public string? NameTr { get; set; }
    public string? Capital { get; set; }
    public string? Region { get; set; }
    public string? Subregion { get; set; }
    public string? Tld { get; set; }
    public string? Flag { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? AreaKm2 { get; set; }
    public bool Independent { get; set; }
    public List<Currency> Currencies { get; set; } = [];
    public List<Language> Languages { get; set; } = [];
    public string? CallingCode { get; set; }
    public bool InEu { get; set; }
    public JsonElement? GeoShape { get; set; }
    public GeoShapeAttribution? GeoShapeAttribution { get; set; }
}

public sealed class GeoShapeAttribution
{
    public string? Source { get; set; }
    public string? Url { get; set; }
    public string? License { get; set; }
}

public sealed class Division
{
    public string Id { get; set; } = "";
    public string Country { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public string? ParentId { get; set; }
    public List<Division> Children { get; set; } = [];
}

public sealed class IpLocation
{
    public string? Ip { get; set; }
    public string? Version { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? RegionCode { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryCodeIso3 { get; set; }
    public string? CountryName { get; set; }
    public string? CountryNameTr { get; set; }
    public string? CountryCapital { get; set; }
    public string? CountryTld { get; set; }
    public string? CountryFlag { get; set; }
    public string? ContinentCode { get; set; }
    public bool? InEu { get; set; }
    public string? Postal { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Timezone { get; set; }
    public string? UtcOffset { get; set; }
    public string? CountryCallingCode { get; set; }
    public string? Currency { get; set; }
    public string? CurrencyName { get; set; }
    public string? Languages { get; set; }
    public List<string> LanguageCodes { get; set; } = [];
    public double? CountryArea { get; set; }
    public string? Asn { get; set; }
    public string? Org { get; set; }
    public string? Colo { get; set; }
    public string? MetroCode { get; set; }
    public string? BrowserLanguage { get; set; }
    public string? Source { get; set; }
    public string? Attribution { get; set; }
    public string? AttributionUrl { get; set; }
    /// <summary>Listed in the Tor exit list. null when the list is not loaded.</summary>
    public bool? IsTor { get; set; }
    /// <summary>Inside a published cloud or hosting range. null when the data is not loaded.</summary>
    public bool? IsHosting { get; set; }
    public string? HostingProvider { get; set; }
}

public sealed class CallingCode
{
    public string Country { get; set; } = "";
    public string Name { get; set; } = "";
    public string CallingCodeValue { get; set; } = "";

    [JsonPropertyName("calling_code")]
    public string CallingCodeJson { get => CallingCodeValue; set => CallingCodeValue = value; }
}

public sealed class PhoneValidation
{
    public bool Valid { get; set; }
    public bool Possible { get; set; }
    public string? Reason { get; set; }
    [JsonPropertyName("e164")]
    public string? E164 { get; set; }
    public string? National { get; set; }
    public string? International { get; set; }
    public string? Country { get; set; }
    public string? CallingCode { get; set; }
    public string? Type { get; set; }
}

public sealed class PlateFormat
{
    public string Country { get; set; } = "";
    public string Description { get; set; } = "";
    public string Example { get; set; } = "";
}

public sealed class PlateValidation
{
    public bool Valid { get; set; }
    public string? Reason { get; set; }
    public string? Normalized { get; set; }
    public string Country { get; set; } = "";
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
}

public sealed class ReferenceRate
{
    public string Code { get; set; } = "";
    public double Rate { get; set; }
}

public sealed class TcmbRate
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Unit { get; set; }
    public double SourceUnit { get; set; }
    public double? ForexBuying { get; set; }
    public double? ForexSelling { get; set; }
    public double? BanknoteBuying { get; set; }
    public double? BanknoteSelling { get; set; }
}

public sealed class Rates
{
    public string Source { get; set; } = "";
    public string Base { get; set; } = "";
    public string? Date { get; set; }
    public string FetchedAt { get; set; } = "";
    public List<ReferenceRate> Reference { get; set; } = [];
    public List<TcmbRate> Tcmb { get; set; } = [];

    internal static Rates FromWire(RatesWire wire, JsonSerializerOptions options)
    {
        var rates = new Rates { Source = wire.Source, Base = wire.Base, Date = wire.Date, FetchedAt = wire.FetchedAt };
        if (wire.Rates.ValueKind == JsonValueKind.Array)
        {
            if (wire.Source == "tcmb") rates.Tcmb = wire.Rates.Deserialize<List<TcmbRate>>(options) ?? [];
            else rates.Reference = wire.Rates.Deserialize<List<ReferenceRate>>(options) ?? [];
        }
        return rates;
    }
}

internal sealed class RatesWire
{
    public string Source { get; set; } = "";
    public string Base { get; set; } = "";
    public string? Date { get; set; }
    public string FetchedAt { get; set; } = "";
    public JsonElement Rates { get; set; }
}

public sealed class Conversion
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public double Amount { get; set; }
    public double Rate { get; set; }
    public double Result { get; set; }
    public string Source { get; set; } = "";
    public string Base { get; set; } = "";
    public string? Date { get; set; }
    public string? TcmbRate { get; set; }
}

public sealed class Place
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Country { get; set; }
    public string? Label { get; set; }
}

public sealed class Distance
{
    public double Kilometers { get; set; }
    public Place From { get; set; } = new();
    public Place To { get; set; } = new();
}

public sealed class IbanValidation
{
    public bool Valid { get; set; }
    public string? Reason { get; set; }
    public string? Iban { get; set; }
    public string? Formatted { get; set; }
    public string? Country { get; set; }
    public string? CheckDigits { get; set; }
    public string? Bban { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
}

public sealed class TrNumberValidation
{
    public bool Valid { get; set; }
    public string? Reason { get; set; }
    public string Type { get; set; } = "";
    public bool RegistryChecked { get; set; }
}

public sealed class Holiday
{
    public string Date { get; set; } = "";
    public string Name { get; set; } = "";
    public string LocalName { get; set; } = "";
    public bool HalfDay { get; set; }
    public bool Nationwide { get; set; }
}

public sealed class HolidayList
{
    public string Country { get; set; } = "";
    public int Year { get; set; }
    public string Source { get; set; } = "";
    public List<Holiday> Holidays { get; set; } = [];
}

public sealed class BusinessDays
{
    public string Country { get; set; } = "";
    public string Start { get; set; } = "";
    public int? Days { get; set; }
    public string End { get; set; } = "";
    public int BusinessDaysCount { get; set; }
    public List<int> Weekend { get; set; } = [];
    public List<string> HolidaysSkipped { get; set; } = [];

    [JsonPropertyName("business_days")]
    public int BusinessDaysJson { get => BusinessDaysCount; set => BusinessDaysCount = value; }
}

public sealed class Timezone
{
    public string TimezoneId { get; set; } = "";
    public string? UtcOffset { get; set; }
    public string? Abbreviation { get; set; }
    public bool IsDst { get; set; }
    public string LocalTime { get; set; } = "";
    public string? CountryCode { get; set; }
    public List<string> Timezones { get; set; } = [];
    public string Source { get; set; } = "";

    [JsonPropertyName("timezone")]
    public string TimezoneJson { get => TimezoneId; set => TimezoneId = value; }
}

public sealed class MxRecord
{
    public int Priority { get; set; }
    public string Exchange { get; set; } = "";
}

public sealed class EmailCheck
{
    public string Email { get; set; } = "";
    public bool Valid { get; set; }
    public string? Reason { get; set; }
    public string? Normalized { get; set; }
    public string? LocalPart { get; set; }
    public string? Domain { get; set; }
    public bool SyntaxValid { get; set; }
    public bool DnsChecked { get; set; }
    public bool? DomainExists { get; set; }
    public bool? AcceptsMail { get; set; }
    public List<MxRecord> MxRecords { get; set; } = [];
    public bool Disposable { get; set; }
    public bool Role { get; set; }
}

public sealed class VatCheck
{
    public string CountryCode { get; set; } = "";
    public string VatNumber { get; set; } = "";
    public bool Valid { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string RequestDate { get; set; } = "";
    public string Source { get; set; } = "";
}

public sealed class IdValidator
{
    public string Country { get; set; } = "";
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string Holder { get; set; } = "";
    public string Description { get; set; } = "";
    public string Example { get; set; } = "";
    public string Path { get; set; } = "";
}

public sealed class IdValidatorList
{
    public List<IdValidator> Data { get; set; } = [];
}

public sealed class IdValidation
{
    public bool Valid { get; set; }
    public string? Normalized { get; set; }
    public string Country { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Reason { get; set; }
    public JsonElement? Details { get; set; }
    public bool RegistryChecked { get; set; }
}

// Public name retained for source compatibility with earlier client releases.
public sealed class GeoNamesAttribution
{
    public string Source { get; set; } = "";
    public string Url { get; set; } = "";
    public string License { get; set; } = "";
}

public sealed class PostalCodeCountry
{
    public string Country { get; set; } = "";
    public int Count { get; set; }
    public bool Partial { get; set; }
    public string Updated { get; set; } = "";
}

public sealed class PostalCodeCountries
{
    public List<PostalCodeCountry> Data { get; set; } = [];
    public GeoNamesAttribution Attribution { get; set; } = new();
}

public sealed class PostalPlace
{
    public string PostalCode { get; set; } = "";
    public string Place { get; set; } = "";
    public string? Admin1 { get; set; }
    [JsonPropertyName("admin1_code")]
    public string? Admin1Code { get; set; }
    public string? Admin2 { get; set; }
    [JsonPropertyName("admin2_code")]
    public string? Admin2Code { get; set; }
    public string? Admin3 { get; set; }
    [JsonPropertyName("admin3_code")]
    public string? Admin3Code { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? Accuracy { get; set; }
}

public sealed class PostalCodeLookup
{
    public string Country { get; set; } = "";
    public string Query { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Match { get; set; } = "";
    public bool PartialData { get; set; }
    public string? CoverageNote { get; set; }
    public List<PostalPlace> Places { get; set; } = [];
    public string Updated { get; set; } = "";
    /// <summary>LiveAPI in-house postal dataset.</summary>
    public GeoNamesAttribution Attribution { get; set; } = new();
}

public sealed class ReverseArea
{
    public string? Code { get; set; }
    public string? Id { get; set; }
    public string Name { get; set; } = "";
    public string? Type { get; set; }
}

public sealed class ReverseGeocode
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CountryCode { get; set; } = "";
    public string? CountryName { get; set; }
    public ReverseArea? Admin1 { get; set; }
    public ReverseArea? Admin2 { get; set; }
    public string Precision { get; set; } = "";
    public string Attribution { get; set; } = "";
}

public sealed class UaNameVersion
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Major { get; set; }
}

public sealed class UaDevice
{
    public string Type { get; set; } = "";
    public string? Vendor { get; set; }
    public string? Model { get; set; }
}

public sealed class UaCpu
{
    public string? Architecture { get; set; }
}

public sealed class UserAgent
{
    public string Ua { get; set; } = "";
    public UaNameVersion Browser { get; set; } = new();
    public UaNameVersion Engine { get; set; } = new();
    public UaNameVersion Os { get; set; } = new();
    public UaDevice Device { get; set; } = new();
    public UaCpu Cpu { get; set; } = new();
    public bool IsBot { get; set; }
    public string? BotMatch { get; set; }
}

public sealed class BarcodeValidation
{
    public string Input { get; set; } = "";
    public string Normalized { get; set; } = "";
    public bool Valid { get; set; }
    public string? Type { get; set; }
    public string? Reason { get; set; }
    public string? CheckDigit { get; set; }
    public string? ExpectedCheckDigit { get; set; }
    [JsonPropertyName("gtin14")]
    public string? Gtin14 { get; set; }
    [JsonPropertyName("isbn10")]
    public string? Isbn10 { get; set; }
    [JsonPropertyName("isbn13")]
    public string? Isbn13 { get; set; }
}

public sealed class Point
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Country { get; set; }

    public static Point FromCountry(string country) => new() { Country = country };
    public static Point FromCoordinates(double latitude, double longitude) => new() { Latitude = latitude, Longitude = longitude };
}
