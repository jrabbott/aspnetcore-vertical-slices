using System.Net;
using AngleSharp.Dom;

namespace WeatherApp.Integration.Tests;

public sealed class FavoritesEndpointTests(WeatherAppFactory factory) : IClassFixture<WeatherAppFactory>
{
    private readonly WeatherAppFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [Fact]
    public async Task Favorites_EmptyStore_ShowsEmptyState()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        HttpResponseMessage response = await client.GetAsync("/weather/favorites");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("You have no favorite cities yet", document.QuerySelector("p.empty")?.TextContent);
        Assert.Equal("Add a favorite", document.QuerySelector("section.add-favorite h2")?.TextContent.Trim());
        Assert.Empty(document.QuerySelectorAll("ul.favorites-list > li"));
    }

    [Fact]
    public async Task Favorites_CityWithoutWeather_ShowsUnavailableMessage()
    {
        HttpClient client = _factory.CreateClientWithFavorites(["Atlantis"]);
        HttpResponseMessage response = await client.GetAsync("/weather/favorites");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            document.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Atlantis");
        Assert.Contains(
            "Weather unavailable for this city.",
            document.QuerySelector("p.muted")?.TextContent);
    }

    [Fact]
    public async Task AddAndRemoveFavorite_RoundTrip()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        HttpResponseMessage page = await client.GetAsync("/weather/favorites");
        IDocument document = await HtmlDocument.ParseAsync(page);
        string token = HtmlDocument.AntiForgeryToken(document);

        using FormUrlEncodedContent addContent = Form(token, "Madrid");
        HttpResponseMessage addResponse = await client.PostAsync("/weather/favorites/add", addContent);

        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);
        Assert.Equal("/weather/favorites", addResponse.Headers.Location?.ToString());

        IDocument afterAdd = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was added", afterAdd.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.Contains(
            afterAdd.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");

        string removeToken = HtmlDocument.AntiForgeryToken(afterAdd);
        using FormUrlEncodedContent removeContent = Form(removeToken, "Madrid");
        HttpResponseMessage removeResponse = await client.PostAsync("/weather/favorites/remove", removeContent);

        Assert.Equal(HttpStatusCode.Redirect, removeResponse.StatusCode);

        IDocument afterRemove = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was removed", afterRemove.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.DoesNotContain(
            afterRemove.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");
    }

    [Fact]
    public async Task AddFavorite_DuplicateCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavorites(["London"]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "London");
        HttpResponseMessage response = await client.PostAsync("/weather/favorites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "already in your favorites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task AddFavorite_UnsupportedCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Atlantis");
        HttpResponseMessage response = await client.PostAsync("/weather/favorites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "not a supported city",
            document.QuerySelector(".alert.alert-error")?.TextContent);
        Assert.Empty(document.QuerySelectorAll("ul.favorites-list > li"));
    }

    [Fact]
    public async Task AddFavorite_BlankCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "");
        HttpResponseMessage response = await client.PostAsync("/weather/favorites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "Please enter a city name.",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task RemoveFavorite_MissingCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Madrid");
        HttpResponseMessage response = await client.PostAsync("/weather/favorites/remove", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "was not in your favorites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task RemoveFavorite_BlankCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavorites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "");
        HttpResponseMessage response = await client.PostAsync("/weather/favorites/remove", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "A city is required to remove a favorite",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    private static FormUrlEncodedContent Form(string token, string city)
    {
        return new(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["City"] = city
        });
    }
}
