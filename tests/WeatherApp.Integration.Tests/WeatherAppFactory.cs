using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.TestSupport;

namespace WeatherApp.Integration.Tests;

public sealed class WeatherAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
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
