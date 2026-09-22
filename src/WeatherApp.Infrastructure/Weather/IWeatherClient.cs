using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

public interface IWeatherClient
{
    Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default);
}
