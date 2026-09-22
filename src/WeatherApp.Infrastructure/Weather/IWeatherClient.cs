using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

public interface IWeatherClient
{
    public Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default);
}
