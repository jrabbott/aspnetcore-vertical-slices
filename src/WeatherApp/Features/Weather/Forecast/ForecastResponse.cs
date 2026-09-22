using WeatherApp.Domain.Weather;

namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastResponse
{
    public ForecastRequest Request { get; init; } = new();
    public bool Searched
    {
        get; init;
    }
    public bool Found
    {
        get; init;
    }
    public string? ErrorMessage
    {
        get; init;
    }
    public string? City
    {
        get; init;
    }
    public string? Country
    {
        get; init;
    }
    public IReadOnlyList<ForecastDay> Days { get; init; } = [];
    public IReadOnlyList<string> ExampleCities { get; init; } = [];

    public static ForecastResponse Empty(ForecastRequest request, IReadOnlyList<string> exampleCities)
    {
        return new()
        {
            Request = request,
            Searched = false,
            ExampleCities = exampleCities
        };
    }

    public static ForecastResponse Invalid(
        ForecastRequest request,
        string errorMessage,
        IReadOnlyList<string> exampleCities)
    {
        return new()
        {
            Request = request,
            Searched = true,
            Found = false,
            ErrorMessage = errorMessage,
            ExampleCities = exampleCities
        };
    }

    public static ForecastResponse NotFound(
        ForecastRequest request,
        string city,
        IReadOnlyList<string> exampleCities)
    {
        ArgumentNullException.ThrowIfNull(city);

        return Invalid(
            request,
            $"No forecast found for \"{city.Trim()}\". Try one of the example cities.",
            exampleCities);
    }

    public static ForecastResponse FromReadings(
        ForecastRequest request,
        IReadOnlyList<WeatherReading> readings,
        IReadOnlyList<string> exampleCities)
    {
        ArgumentNullException.ThrowIfNull(readings);

        return new()
        {
            Request = request,
            Searched = true,
            Found = true,
            City = readings[0].Location.City,
            Country = readings[0].Location.Country,
            Days = readings.Select(ForecastDay.FromReading).ToArray(),
            ExampleCities = exampleCities
        };
    }
}

public sealed class ForecastDay
{
    public required DateOnly Date
    {
        get; init;
    }
    public required int TemperatureC
    {
        get; init;
    }
    public required int TemperatureF
    {
        get; init;
    }
    public required string Summary
    {
        get; init;
    }
    public required int HumidityPercent
    {
        get; init;
    }
    public required int WindSpeedKph
    {
        get; init;
    }

    public static ForecastDay FromReading(WeatherReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);

        return new()
        {
            Date = reading.Date,
            TemperatureC = reading.TemperatureC,
            TemperatureF = reading.TemperatureF,
            Summary = reading.Summary,
            HumidityPercent = reading.HumidityPercent,
            WindSpeedKph = reading.WindSpeedKph
        };
    }
}
