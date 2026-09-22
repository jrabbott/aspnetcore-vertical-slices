using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class ForecastEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client;

    public ForecastEndpointTests(WeatherAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Forecast_KnownCity_RendersMultiDayList()
    {
        var response = await _client.GetAsync("/weather/forecast?city=Paris&days=3");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Paris, France", html);
        Assert.Contains("forecast-list", html);
        Assert.Contains("Humidity", html);
    }

    [Fact]
    public async Task Forecast_UnknownCity_ShowsError()
    {
        var response = await _client.GetAsync("/weather/forecast?city=Nowhere");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("No forecast found", html);
    }
}
