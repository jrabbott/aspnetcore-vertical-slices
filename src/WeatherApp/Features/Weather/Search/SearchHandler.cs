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
            return SearchResponse.Empty(request, exampleCities);
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return SearchResponse.Invalid(request, validation.Errors[0].ErrorMessage, exampleCities);
        }

        var reading = await _weatherClient.GetCurrentAsync(request.City!, cancellationToken);
        if (reading is null)
        {
            return SearchResponse.NotFound(request, request.City!, exampleCities);
        }

        return SearchResponse.FromReading(request, reading, exampleCities);
    }
}
