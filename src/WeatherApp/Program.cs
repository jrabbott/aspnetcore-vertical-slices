using WeatherApp.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeatherApp(builder.Configuration);

WebApplication app = builder.Build();

app.UseWeatherApp();

app.Run();
