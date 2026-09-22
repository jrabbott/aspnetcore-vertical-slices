using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class HomeEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly WeatherAppFactory _factory;

    public HomeEndpointTests(WeatherAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Error_RendersErrorPageWithRequestId()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Home/Error");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Something went wrong", document.QuerySelector("h1")?.TextContent.Trim());
        Assert.Contains("An error occurred", document.QuerySelector(".page-header p")?.TextContent);
        Assert.False(string.IsNullOrWhiteSpace(document.QuerySelector("code")?.TextContent));
    }

    [Fact]
    public async Task Production_HostsAppAndServesSearch()
    {
        var client = _factory.CreateProductionClient();
        var response = await client.GetAsync("/weather/search");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Search weather", document.QuerySelector("h1")?.TextContent.Trim());
    }
}
