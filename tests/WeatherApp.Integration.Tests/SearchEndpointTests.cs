using System.Net;
using AngleSharp.Dom;

namespace WeatherApp.Integration.Tests;

public sealed class SearchEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client;

    public SearchEndpointTests(WeatherAppFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Root_RedirectsToSearch()
    {
        HttpResponseMessage response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/weather/search", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Search_RendersFeatureViewAndLayout()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/search");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Search weather", document.QuerySelector("h1")?.TextContent.Trim());
        Assert.Equal("WeatherApp", document.QuerySelector("a.brand")?.TextContent.Trim());
        Assert.NotNull(document.QuerySelector("nav.site-nav a[href='/weather/forecast']"));
        Assert.NotNull(document.QuerySelector("nav.site-nav a[href='/weather/favorites']"));
        Assert.NotNull(document.QuerySelector("link[href*='site.css']"));
        Assert.NotNull(document.QuerySelector("form.search-form input[name='City']"));
    }

    [Fact]
    public async Task Search_KnownCity_ShowsCurrentWeather()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/search?city=London");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IElement? result = document.QuerySelector("article.weather-result");
        Assert.NotNull(result);
        Assert.Equal("London, United Kingdom", result.QuerySelector("h2")?.TextContent.Trim());
        Assert.False(string.IsNullOrWhiteSpace(result.QuerySelector("p.summary")?.TextContent));
        Assert.Contains("°C", result.TextContent);
        Assert.Contains("°F", result.TextContent);
        Assert.NotNull(result.QuerySelector("a[href='/weather/forecast?city=London']"));
    }

    [Fact]
    public async Task Search_UnknownCity_ShowsError()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/search?city=Zzqxnotacity999");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IElement? alert = document.QuerySelector(".alert.alert-error");
        Assert.NotNull(alert);
        Assert.Contains("No weather data found", alert.TextContent);
        Assert.Contains("Zzqxnotacity999", alert.TextContent);
        Assert.Null(document.QuerySelector("article.weather-result"));
    }

    [Fact]
    public async Task Search_BlankCity_ShowsFieldError()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/search?city=");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "Please enter a city name.",
            document.QuerySelector(".field-error")?.TextContent.Trim());
        Assert.Contains(
            "Please enter a city name.",
            document.QuerySelector(".alert.alert-error")?.TextContent);
        Assert.Null(document.QuerySelector("article.weather-result"));
    }
}
