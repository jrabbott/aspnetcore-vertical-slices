namespace WeatherApp.Infrastructure.Weather;

/// <summary>
/// HTTP base URLs for Open-Meteo geocoding and forecast APIs.
/// Override in tests or containers (for example WireMock) via the <c>OpenMeteo</c> configuration section.
/// </summary>
public sealed class OpenMeteoOptions
{
    public const string SectionName = "OpenMeteo";

    public const string DefaultGeocodingBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";

    public const string DefaultForecastBaseUrl = "https://api.open-meteo.com/v1/forecast";

    public string GeocodingBaseUrl { get; set; } = DefaultGeocodingBaseUrl;

    public string ForecastBaseUrl { get; set; } = DefaultForecastBaseUrl;
}
