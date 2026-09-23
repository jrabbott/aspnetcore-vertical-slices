using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.Infrastructure.Weather;
using WeatherApp.TestSupport;

namespace WeatherApp.Integration.Tests;

public sealed class WeatherAppFactory : WebApplicationFactory<Program>
{
    private static readonly FakeWeatherClient _fakeWeather = new(
        FakeWeatherClient.Reading("London", "United Kingdom", 12, "Cloudy"),
        FakeWeatherClient.Reading("Paris", "France", 18, "Partly cloudy"),
        FakeWeatherClient.Reading("Madrid", "Spain", 22, "Sunny"),
        FakeWeatherClient.Reading("Tokyo", "Japan", 20, "Humid"),
        FakeWeatherClient.Reading("New York", "United States", 15, "Clear"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            // Integration tests stay offline: no live Open-Meteo calls.
            services.RemoveAll<IWeatherClient>();
            services.AddSingleton<IWeatherClient>(_fakeWeather);
        });
    }

    public HttpClient CreateClientWithFavourites(
        IEnumerable<string>? initialCities = null,
        WebApplicationFactoryClientOptions? options = null)
    {
        string[] cities = initialCities?.ToArray() ?? [];
        WebApplicationFactoryClientOptions clientOptions = options ?? new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        // Empty list: use real session-backed store (cookie jar isolates browsers).
        if (cities.Length == 0)
        {
            return CreateClient(clientOptions);
        }

        // Non-empty seed (including ungeocodable cities): deterministic in-memory fake.
        return WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IFavouritesStore>();
                services.AddSingleton<IFavouritesStore>(_ => new FakeFavouritesStore(cities));
            })).CreateClient(clientOptions);
    }

    public HttpClient CreateProductionClient(WebApplicationFactoryClientOptions? options = null)
    {
        return WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient(options ?? new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
