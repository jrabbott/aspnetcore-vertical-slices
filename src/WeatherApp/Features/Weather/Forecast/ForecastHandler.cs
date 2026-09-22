using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastHandler(IWeatherClient weatherClient, IValidator<ForecastRequest> validator)
{
    private readonly IWeatherClient _weatherClient = weatherClient;
    private readonly IValidator<ForecastRequest> _validator = validator;

    public async Task<ForecastResponse> HandleAsync(
        ForecastRequest request,
        bool searched,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string[] exampleCities = [.. WeatherClient.KnownCities];

        if (!searched)
        {
            return ForecastResponse.Empty(request, exampleCities);
        }

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ForecastResponse.Invalid(request, validation.Errors[0].ErrorMessage, exampleCities);
        }

        IReadOnlyList<WeatherReading> readings = await _weatherClient.GetForecastAsync(request.City!, request.Days, cancellationToken);
        return readings.Count == 0
            ? ForecastResponse.NotFound(request, request.City!, exampleCities)
            : ForecastResponse.FromReadings(request, readings, exampleCities);
    }
}
