using System.Net;
using System.Text.RegularExpressions;

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
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("You have no favorite cities yet", html);
        Assert.Contains("Add a favorite", html);
    }

    [Fact]
    public async Task AddAndRemoveFavorite_RoundTrip()
    {
        var page = await _client.GetAsync("/weather/favorites");
        var html = await page.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);

        var addResponse = await _client.PostAsync(
            "/weather/favorites/add",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = token,
                ["City"] = "Madrid"
            }));

        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);
        Assert.Equal("/weather/favorites", addResponse.Headers.Location?.ToString());

        var afterAdd = await _client.GetAsync("/weather/favorites");
        var afterAddHtml = await afterAdd.Content.ReadAsStringAsync();
        Assert.Contains("<h2>Madrid</h2>", afterAddHtml);
        Assert.Contains("Madrid was added", afterAddHtml);

        var removeToken = ExtractAntiForgeryToken(afterAddHtml);
        var removeResponse = await _client.PostAsync(
            "/weather/favorites/remove",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = removeToken,
                ["City"] = "Madrid"
            }));

        Assert.Equal(HttpStatusCode.Redirect, removeResponse.StatusCode);

        var afterRemove = await _client.GetAsync("/weather/favorites");
        var afterRemoveHtml = await afterRemove.Content.ReadAsStringAsync();
        Assert.Contains("Madrid was removed", afterRemoveHtml);
        Assert.DoesNotContain("<h2>Madrid</h2>", afterRemoveHtml);
    }

    private static string ExtractAntiForgeryToken(string html)
    {
        var match = Regex.Match(
            html,
            """name="__RequestVerificationToken"[^>]*value="([^"]+)""",
            RegexOptions.IgnoreCase);

        Assert.True(match.Success, "Anti-forgery token was not found in the page.");
        return match.Groups[1].Value;
    }
}
