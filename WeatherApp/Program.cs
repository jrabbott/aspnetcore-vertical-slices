using WeatherApp.Features.Weather.AddFavorite;
using WeatherApp.Features.Weather.Favorites;
using WeatherApp.Features.Weather.Forecast;
using WeatherApp.Features.Weather.RemoveFavorite;
using WeatherApp.Features.Weather.Search;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Razor;
using WeatherApp.Infrastructure.Weather;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
});

builder.Services.AddSingleton<IWeatherClient, WeatherClient>();
builder.Services.AddSingleton<IFavoritesStore, FavoritesStore>();

builder.Services.AddTransient<SearchHandler>();
builder.Services.AddTransient<ForecastHandler>();
builder.Services.AddTransient<FavoritesHandler>();
builder.Services.AddTransient<AddFavoriteHandler>();
builder.Services.AddTransient<RemoveFavoriteHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/", () => Results.Redirect("/weather/search"));

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program;
