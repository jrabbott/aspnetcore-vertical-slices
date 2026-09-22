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
            return new ForecastResponse
            {
                Request = request,
                Searched = false,
                ExampleCities = exampleCities
            };
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return new ForecastResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = validation.Errors[0].ErrorMessage,
                ExampleCities = exampleCities
            };
        }

        var readings = await _weatherClient.GetForecastAsync(request.City!, request.Days, cancellationToken);

        if (readings.Count == 0)
        {
            return new ForecastResponse
            {
                Request = request,
                Searched = true,
                Found = false,
                ErrorMessage = $"No forecast found for \"{request.City!.Trim()}\". Try one of the example cities.",
                ExampleCities = exampleCities
            };
        }

        return new ForecastResponse
        {
            Request = request,
            Searched = true,
            Found = true,
            City = readings[0].Location.City,
            Country = readings[0].Location.Country,
            Days = readings.Select(r => new ForecastDay
            {
                Date = r.Date,
                TemperatureC = r.TemperatureC,
                TemperatureF = r.TemperatureF,
                Summary = r.Summary,
                HumidityPercent = r.HumidityPercent,
                WindSpeedKph = r.WindSpeedKph
            }).ToArray(),
            ExampleCities = exampleCities
        };
    }
}
