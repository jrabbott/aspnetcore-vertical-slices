using FluentValidation;
using Microsoft.AspNetCore.Mvc.Razor;
using WeatherApp.Features.Weather.AddFavorite;
using WeatherApp.Features.Weather.Favorites;
using WeatherApp.Features.Weather.Forecast;
using WeatherApp.Features.Weather.RemoveFavorite;
using WeatherApp.Features.Weather.Search;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;
using WeatherApp.Razor;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new FeatureViewLocationExpander()));

builder.Services.AddSingleton<IWeatherClient, WeatherClient>();
builder.Services.AddSingleton<IFavoritesStore, FavoritesStore>();

builder.Services.AddTransient<IValidator<SearchRequest>, SearchRequestValidator>();
builder.Services.AddTransient<IValidator<ForecastRequest>, ForecastRequestValidator>();
builder.Services.AddTransient<IValidator<AddFavoriteRequest>, AddFavoriteRequestValidator>();
builder.Services.AddTransient<IValidator<RemoveFavoriteRequest>, RemoveFavoriteRequestValidator>();

builder.Services.AddTransient<SearchHandler>();
builder.Services.AddTransient<ForecastHandler>();
builder.Services.AddTransient<FavoritesHandler>();
builder.Services.AddTransient<AddFavoriteHandler>();
builder.Services.AddTransient<RemoveFavoriteHandler>();

WebApplication app = builder.Build();

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
