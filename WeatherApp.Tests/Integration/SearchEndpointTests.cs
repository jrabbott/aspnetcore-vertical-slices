using System.Net;

namespace WeatherApp.Tests.Integration;

public sealed class SearchEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client;

    public SearchEndpointTests(WeatherAppFactory factory)
    {
        _client = factory.CreateClient(new() { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Root_RedirectsToSearch()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/weather/search", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Search_RendersFeatureViewAndLayout()
    {
        var response = await _client.GetAsync("/weather/search");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Search weather", html);
        Assert.Contains("WeatherApp", html);
        Assert.Contains("/weather/forecast", html);
        Assert.Contains("/weather/favorites", html);
        Assert.Contains("site.css", html);
    }

    [Fact]
    public async Task Search_KnownCity_ShowsCurrentWeather()
    {
        var response = await _client.GetAsync("/weather/search?city=London");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("London, United Kingdom", html);
        Assert.Contains("&deg;C", html);
    }

    [Fact]
    public async Task Search_UnknownCity_ShowsError()
    {
        var response = await _client.GetAsync("/weather/search?city=Atlantis");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("No weather data found", html);
        Assert.Contains("Atlantis", html);
    }
}
