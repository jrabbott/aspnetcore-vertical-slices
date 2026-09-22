using FluentValidation;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastHandler
{
    private readonly IWeatherClient _weatherClient;
    private readonly IValidator<ForecastRequest> _validator;

    public ForecastHandler(IWeatherClient weatherClient, IValidator<ForecastRequest> validator)
    {
        _weatherClient = weatherClient;
        _validator = validator;
    }

    public async Task<ForecastResponse> HandleAsync(
        ForecastRequest request,
        bool searched,
        CancellationToken cancellationToken = default)
    {
        var exampleCities = WeatherClient.KnownCities.ToArray();

        if (!searched)
        {
            return ForecastResponse.Empty(request, exampleCities);
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ForecastResponse.Invalid(request, validation.Errors[0].ErrorMessage, exampleCities);
        }

        var readings = await _weatherClient.GetForecastAsync(request.City!, request.Days, cancellationToken);
        if (readings.Count == 0)
        {
            return ForecastResponse.NotFound(request, request.City!, exampleCities);
        }

        return ForecastResponse.FromReadings(request, readings, exampleCities);
    }
}
