namespace WeatherApp.Hosting;

internal static class WeatherAppApplicationBuilderExtensions
{
    public static WebApplication UseWeatherAppExceptionHandling(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        return app;
    }

    public static WebApplication UseWeatherAppRequestPipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();
        app.MapStaticAssets();
        return app;
    }

    public static WebApplication MapWeatherAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/weather/search"));
        app.MapHealthChecks("/health");

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        return app;
    }
}
