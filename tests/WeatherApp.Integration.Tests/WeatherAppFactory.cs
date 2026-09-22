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
}
