using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Weather;

internal sealed class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeocodingResult>? Results
    {
        get; init;
    }
}

internal sealed class GeocodingResult
{
    [JsonPropertyName("name")]
    public string Name
    {
        get; init;
    } = string.Empty;

    [JsonPropertyName("country")]
    public string? Country
    {
        get; init;
    }

    [JsonPropertyName("latitude")]
    public double Latitude
    {
        get; init;
    }

    [JsonPropertyName("longitude")]
    public double Longitude
    {
        get; init;
    }
}

internal sealed class ForecastResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current
    {
        get; init;
    }

    [JsonPropertyName("daily")]
    public DailyWeather? Daily
    {
        get; init;
    }
}

internal sealed class CurrentWeather
{
    [JsonPropertyName("time")]
    public string? Time
    {
        get; init;
    }

    [JsonPropertyName("temperature_2m")]
    public double Temperature2M
    {
        get; init;
    }

    [JsonPropertyName("relative_humidity_2m")]
    public int RelativeHumidity2M
    {
        get; init;
    }

    [JsonPropertyName("weather_code")]
    public int WeatherCode
    {
        get; init;
    }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed10M
    {
        get; init;
    }
}

internal sealed class DailyWeather
{
    [JsonPropertyName("time")]
    public List<string>? Time
    {
        get; init;
    }

    [JsonPropertyName("weather_code")]
    public List<double>? WeatherCode
    {
        get; init;
    }

    [JsonPropertyName("temperature_2m_max")]
    public List<double>? Temperature2MMax
    {
        get; init;
    }

    [JsonPropertyName("relative_humidity_2m_mean")]
    public List<double>? RelativeHumidity2MMean
    {
        get; init;
    }

    [JsonPropertyName("wind_speed_10m_max")]
    public List<double>? WindSpeed10MMax
    {
        get; init;
    }
}
