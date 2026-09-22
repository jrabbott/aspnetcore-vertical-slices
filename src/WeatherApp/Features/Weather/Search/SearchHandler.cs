using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchHandler(IWeatherClient weatherClient, IValidator<SearchRequest> validator)
{
    private readonly IWeatherClient _weatherClient = weatherClient;
    private readonly IValidator<SearchRequest> _validator = validator;

    public async Task<SearchResponse> HandleAsync(
        SearchRequest request,
        bool searched,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string[] exampleCities = [.. WeatherClient.KnownCities];

        if (!searched)
        {
            return SearchResponse.Empty(request, exampleCities);
        }

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return SearchResponse.Invalid(request, validation.Errors[0].ErrorMessage, exampleCities);
        }

        WeatherReading? reading = await _weatherClient.GetCurrentAsync(request.City!, cancellationToken);
        return reading is null
            ? SearchResponse.NotFound(request, request.City!, exampleCities)
            : SearchResponse.FromReading(request, reading, exampleCities);
    }
}
