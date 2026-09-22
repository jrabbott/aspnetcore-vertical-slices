using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchHandler
{
    private readonly IWeatherClient _weatherClient;

    public SearchHandler(IWeatherClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    public async Task<SearchResponse> HandleAsync(
        SearchRequest request,
        bool searched,
        CancellationToken cancellationToken = default)
    {
        var exampleCities = WeatherClient.KnownCities.ToArray();

        if (!searched)
        {
            return new SearchResponse
            {
                Request = request,
                Searched = false,
                ExampleCities = exampleCities
            };
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            return new SearchResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = "Please enter a city name.",
                ExampleCities = exampleCities
            };
        }

        var reading = await _weatherClient.GetCurrentAsync(request.City, cancellationToken);

        if (reading is null)
        {
            return new SearchResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = $"No weather data found for \"{request.City.Trim()}\". Try one of the example cities.",
                ExampleCities = exampleCities
            };
        }

        return new SearchResponse
        {
            Request = request,
            Searched = true,
            Found = true,
            City = reading.Location.City,
            Country = reading.Location.Country,
            TemperatureC = reading.TemperatureC,
            TemperatureF = reading.TemperatureF,
            Summary = reading.Summary,
            HumidityPercent = reading.HumidityPercent,
            WindSpeedKph = reading.WindSpeedKph,
            ExampleCities = exampleCities
        };
    }
}
