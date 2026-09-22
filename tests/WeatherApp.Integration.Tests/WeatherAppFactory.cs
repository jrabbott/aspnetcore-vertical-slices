using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Integration.Tests;

public sealed class WeatherAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IFavoritesStore>();
            services.AddSingleton<IFavoritesStore>(_ => new FavoritesStore([]));
        });
    }

    public HttpClient CreateClientWithFavorites(
        IEnumerable<string>? initialCities = null,
        WebApplicationFactoryClientOptions? options = null)
    {
        string[] cities = initialCities?.ToArray() ?? [];

        return WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IFavoritesStore>();
                services.AddSingleton<IFavoritesStore>(_ => new FavoritesStore(cities));
            })).CreateClient(options ?? new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    public HttpClient CreateProductionClient(WebApplicationFactoryClientOptions? options = null)
    {
        return WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient(options ?? new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
