using FluentValidation;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchHandler
{
    private readonly IWeatherClient _weatherClient;
    private readonly IValidator<SearchRequest> _validator;

    public SearchHandler(IWeatherClient weatherClient, IValidator<SearchRequest> validator)
    {
        _weatherClient = weatherClient;
        _validator = validator;
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

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return new SearchResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = validation.Errors[0].ErrorMessage,
                ExampleCities = exampleCities
            };
        }

        var reading = await _weatherClient.GetCurrentAsync(request.City!, cancellationToken);

        if (reading is null)
        {
            return new SearchResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = $"No weather data found for \"{request.City!.Trim()}\". Try one of the example cities.",
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
