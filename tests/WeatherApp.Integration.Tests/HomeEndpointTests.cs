using System.Net;
using AngleSharp.Dom;

namespace WeatherApp.Integration.Tests;

public sealed class HomeEndpointTests(WeatherAppFactory factory) : IClassFixture<WeatherAppFactory>
{
    private readonly WeatherAppFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [Fact]
    public async Task Error_RendersErrorPageWithRequestId()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/Home/Error");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Something went wrong", document.QuerySelector("h1")?.TextContent.Trim());
        Assert.Contains("An error occurred", document.QuerySelector(".page-header p")?.TextContent);
        Assert.False(string.IsNullOrWhiteSpace(document.QuerySelector("code")?.TextContent));
    }

    [Fact]
    public async Task Production_HostsAppAndServesSearch()
    {
        HttpClient client = _factory.CreateProductionClient();
        HttpResponseMessage response = await client.GetAsync("/weather/search");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Search weather", document.QuerySelector("h1")?.TextContent.Trim());
    }
}
