# Testing a feature slice

Load this when adding or changing tests for a slice. Project tables and CI commands: [docs/reference/testing.md](../../../../docs/reference/testing.md), [docs/how-to/run-build-and-test.md](../../../../docs/how-to/run-build-and-test.md).

## Where tests go

| Kind | Project |
|---|---|
| Handler / FluentValidation | `tests/WeatherApp.Unit.Tests` |
| HTTP routes, redirects, HTML | `tests/WeatherApp.Integration.Tests` |
| Slice isolation lists | `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` |

## Patterns

- **Unit:** use `FakeWeatherClient` / `FakeFavouritesStore` from `WeatherApp.TestSupport` when the slice touches weather or favourites. Do not call live Open-Meteo.
- **Integration:** `WeatherAppFactory` + AngleSharp (`HtmlDocument.ParseAsync`). Weather is a seeded `FakeWeatherClient` (no live Open-Meteo). Separate `HttpClient` instances for session isolation.
- **Architecture:** add the new namespace to slice lists / InlineData when slices change.

## Gotchas

- Prefer ungeocodable sentinel cities such as `Zzqxnotacity999` (names like Atlantis/Nowhere can still geocode).
- Match UK spelling in UI assertions.
- Scrape the antiforgery token before POSTing forms.
- Respect `_` private-field naming; CI treats warnings as errors when `CI=true`.

## AngleSharp snippets

```csharp
HttpClient client = _factory.CreateClient(new() { AllowAutoRedirect = false });
// or: _factory.CreateClientWithFavourites(["London"]);

HttpResponseMessage response = await client.GetAsync("/weather/search?city=London");
IDocument document = await HtmlDocument.ParseAsync(response);

Assert.Equal(HttpStatusCode.OK, response.StatusCode);
Assert.NotNull(document.QuerySelector("nav.site-nav a[href='/weather/favourites']"));
```

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
