using WeatherApp.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeatherApp();

WebApplication app = builder.Build();

app.UseWeatherApp();

app.Run();
