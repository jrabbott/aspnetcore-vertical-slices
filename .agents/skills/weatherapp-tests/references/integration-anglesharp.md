# Integration tests with AngleSharp

Pattern used in `tests/WeatherApp.Integration.Tests`:

```csharp
HttpClient client = _factory.CreateClient(new() { AllowAutoRedirect = false });
// or: _factory.CreateClientWithFavourites(["London"]);

HttpResponseMessage response = await client.GetAsync("/weather/search?city=London");
IDocument document = await HtmlDocument.ParseAsync(response);

Assert.Equal(HttpStatusCode.OK, response.StatusCode);
Assert.NotNull(document.QuerySelector("nav.site-nav a[href='/weather/favourites']"));
```

## Forms + antiforgery

```csharp
IDocument page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favourites"));
string token = HtmlDocument.AntiForgeryToken(page);

using FormUrlEncodedContent content = new(new Dictionary<string, string>
{
    ["__RequestVerificationToken"] = token,
    ["City"] = "Madrid"
});
HttpResponseMessage post = await client.PostAsync("/weather/favourites/add", content);
Assert.Equal(HttpStatusCode.Redirect, post.StatusCode);
```

## Session isolation

Use two `CreateClient` instances (not two requests on one client) when asserting favourites do not leak across browsers.
