using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class FavoritesEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client;

    public FavoritesEndpointTests(WeatherAppFactory factory)
    {
        _client = factory.CreateClient(new() { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Favorites_EmptyStore_ShowsEmptyState()
    {
        var response = await _client.GetAsync("/weather/favorites");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("You have no favorite cities yet", document.QuerySelector("p.empty")?.TextContent);
        Assert.Equal("Add a favorite", document.QuerySelector("section.add-favorite h2")?.TextContent.Trim());
        Assert.Empty(document.QuerySelectorAll("ul.favorites-list > li"));
    }

    [Fact]
    public async Task AddAndRemoveFavorite_RoundTrip()
    {
        var page = await _client.GetAsync("/weather/favorites");
        var document = await HtmlDocument.ParseAsync(page);
        var token = HtmlDocument.AntiForgeryToken(document);

        var addResponse = await _client.PostAsync(
            "/weather/favorites/add",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = token,
                ["City"] = "Madrid"
            }));

        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);
        Assert.Equal("/weather/favorites", addResponse.Headers.Location?.ToString());

        var afterAdd = await HtmlDocument.ParseAsync(await _client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was added", afterAdd.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.Contains(
            afterAdd.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");

        var removeToken = HtmlDocument.AntiForgeryToken(afterAdd);
        var removeResponse = await _client.PostAsync(
            "/weather/favorites/remove",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = removeToken,
                ["City"] = "Madrid"
            }));

        Assert.Equal(HttpStatusCode.Redirect, removeResponse.StatusCode);

        var afterRemove = await HtmlDocument.ParseAsync(await _client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was removed", afterRemove.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.DoesNotContain(
            afterRemove.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");
    }
}
