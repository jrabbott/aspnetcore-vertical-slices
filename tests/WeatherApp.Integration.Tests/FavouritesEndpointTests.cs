using System.Net;
using AngleSharp.Dom;

namespace WeatherApp.Integration.Tests;

public sealed class FavouritesEndpointTests(WeatherAppFactory factory) : IClassFixture<WeatherAppFactory>
{
    private readonly WeatherAppFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [Fact]
    public async Task Favourites_AreIsolatedPerBrowserSession()
    {
        HttpClient clientA = _factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
        HttpClient clientB = _factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        IDocument pageA = await HtmlDocument.ParseAsync(await clientA.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(pageA);

        using FormUrlEncodedContent content = Form(token, "Madrid");
        HttpResponseMessage addResponse = await clientA.PostAsync("/weather/favourites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);

        IDocument afterAddA = await HtmlDocument.ParseAsync(await clientA.GetAsync("/weather/favourites"));
        Assert.Contains(
            afterAddA.QuerySelectorAll("ul.favourites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");

        IDocument pageB = await HtmlDocument.ParseAsync(await clientB.GetAsync("/weather/favourites"));
        Assert.Contains("You have no favourite cities yet", pageB.QuerySelector("p.empty")?.TextContent);
        Assert.Empty(pageB.QuerySelectorAll("ul.favourites-list > li"));
    }

    [Fact]
    public async Task Favourites_EmptyStore_ShowsEmptyState()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        HttpResponseMessage response = await client.GetAsync("/weather/favourites");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("You have no favourite cities yet", document.QuerySelector("p.empty")?.TextContent);
        Assert.Equal("Add a favourite", document.QuerySelector("section.add-favourite h2")?.TextContent.Trim());
        Assert.Empty(document.QuerySelectorAll("ul.favourites-list > li"));
    }

    [Fact]
    public async Task Favourites_CityWithoutWeather_ShowsUnavailableMessage()
    {
        HttpClient client = _factory.CreateClientWithFavourites(["Zzqxnotacity999"]);
        HttpResponseMessage response = await client.GetAsync("/weather/favourites");
        IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            document.QuerySelectorAll("ul.favourites-list > li h2"),
            heading => heading.TextContent.Trim() == "Zzqxnotacity999");
        Assert.Contains(
            "Weather unavailable for this city.",
            document.QuerySelector("p.muted")?.TextContent);
    }

    [Fact]
    public async Task AddAndRemoveFavourite_RoundTrip()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        HttpResponseMessage page = await client.GetAsync("/weather/favourites");
        IDocument document = await HtmlDocument.ParseAsync(page);
        string token = HtmlDocument.AntiForgeryToken(document);

        using FormUrlEncodedContent addContent = Form(token, "Madrid");
        HttpResponseMessage addResponse = await client.PostAsync("/weather/favourites/add", addContent);

        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);
        Assert.Equal("/weather/favourites", addResponse.Headers.Location?.ToString());

        IDocument afterAdd = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains("Madrid was added", afterAdd.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.Contains(
            afterAdd.QuerySelectorAll("ul.favourites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");

        string removeToken = HtmlDocument.AntiForgeryToken(afterAdd);
        using FormUrlEncodedContent removeContent = Form(removeToken, "Madrid");
        HttpResponseMessage removeResponse = await client.PostAsync("/weather/favourites/remove", removeContent);

        Assert.Equal(HttpStatusCode.Redirect, removeResponse.StatusCode);

        IDocument afterRemove = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains("Madrid was removed", afterRemove.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.DoesNotContain(
            afterRemove.QuerySelectorAll("ul.favourites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");
    }

    [Fact]
    public async Task AddFavourite_DuplicateCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavourites(["London"]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "London");
        HttpResponseMessage response = await client.PostAsync("/weather/favourites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            "already in your favourites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task AddFavourite_UnsupportedCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Zzqxnotacity999");
        HttpResponseMessage response = await client.PostAsync("/weather/favourites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            "Could not find weather",
            document.QuerySelector(".alert.alert-error")?.TextContent);
        Assert.Empty(document.QuerySelectorAll("ul.favourites-list > li"));
    }

    [Fact]
    public async Task AddFavourite_BlankCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "");
        HttpResponseMessage response = await client.PostAsync("/weather/favourites/add", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            "Please enter a city name.",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task RemoveFavourite_MissingCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Madrid");
        HttpResponseMessage response = await client.PostAsync("/weather/favourites/remove", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            "was not in your favourites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task RemoveFavourite_BlankCity_ShowsError()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "");
        HttpResponseMessage response = await client.PostAsync("/weather/favourites/remove", content);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            "A city is required to remove a favourite",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task AddFavourite_WithJsonAccept_ReturnsJsonWithoutRedirect()
    {
        HttpClient client = _factory.CreateClientWithFavourites([]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Madrid");
        using HttpRequestMessage request = new(HttpMethod.Post, "/weather/favourites/add")
        {
            Content = content
        };
        request.Headers.Accept.ParseAdd("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("\"succeeded\":true", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Madrid was added", body);

        IDocument afterAdd = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains(
            afterAdd.QuerySelectorAll("ul.favourites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");
        Assert.Null(afterAdd.QuerySelector(".alert"));
    }

    [Fact]
    public async Task RemoveFavourite_WithJsonAccept_ReturnsJsonWithoutRedirect()
    {
        HttpClient client = _factory.CreateClientWithFavourites(["Madrid"]);
        IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        string token = HtmlDocument.AntiForgeryToken(page);

        using FormUrlEncodedContent content = Form(token, "Madrid");
        using HttpRequestMessage request = new(HttpMethod.Post, "/weather/favourites/remove")
        {
            Content = content
        };
        request.Headers.Accept.ParseAdd("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"succeeded\":true", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Madrid was removed", body);

        IDocument afterRemove = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
        Assert.Contains("You have no favourite cities yet", afterRemove.QuerySelector("p.empty")?.TextContent);
        Assert.Null(afterRemove.QuerySelector(".alert"));
    }

    [Fact]
    public async Task Favourites_Page_MarksCommandFormsForProgressiveEnhancement()
    {
        HttpClient client = _factory.CreateClientWithFavourites(["London"]);
        IDocument document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));

        Assert.NotEmpty(document.QuerySelectorAll("form[data-enhance='favourite-command'][data-on-success='reload']"));
        Assert.NotEmpty(document.QuerySelectorAll("form[data-enhance='favourite-command'][data-on-success='remove-row']"));
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
