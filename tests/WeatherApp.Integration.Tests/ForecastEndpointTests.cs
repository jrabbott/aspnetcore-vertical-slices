using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class ForecastEndpointTests(WeatherAppFactory factory) : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client = (factory ?? throw new ArgumentNullException(nameof(factory))).CreateClient();

    [Fact]
    public async Task Forecast_WithoutCity_ShowsEmptyForm()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/forecast");
        AngleSharp.Dom.IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Forecast", document.QuerySelector("h1")?.TextContent.Trim());
        Assert.NotNull(document.QuerySelector("form.search-form input[name='City']"));
        Assert.Null(document.QuerySelector("article.forecast-result"));
        Assert.Null(document.QuerySelector(".alert.alert-error"));
        Assert.Null(document.QuerySelector(".field-error"));
    }

    [Fact]
    public async Task Forecast_KnownCity_RendersMultiDayList()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/forecast?city=Paris&days=3");
        AngleSharp.Dom.IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        AngleSharp.Dom.IElement? result = document.QuerySelector("article.forecast-result");
        Assert.NotNull(result);
        Assert.Equal("Paris, France", result.QuerySelector("h2")?.TextContent.Trim());

        AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement> days = result.QuerySelectorAll("ul.forecast-list > li");
        Assert.Equal(3, days.Length);
        Assert.All(days, day =>
        {
            Assert.False(string.IsNullOrWhiteSpace(day.QuerySelector(".forecast-date")?.TextContent));
            Assert.False(string.IsNullOrWhiteSpace(day.QuerySelector(".forecast-summary")?.TextContent));
            Assert.Contains("°C", day.QuerySelector(".forecast-temp")?.TextContent ?? string.Empty);
            Assert.Contains("Humidity", day.QuerySelector(".forecast-meta")?.TextContent ?? string.Empty);
        });
    }

    [Fact]
    public async Task Forecast_UnknownCity_ShowsError()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/forecast?city=Zzqxnotacity999");
        AngleSharp.Dom.IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        AngleSharp.Dom.IElement? alert = document.QuerySelector(".alert.alert-error");
        Assert.NotNull(alert);
        Assert.Contains("No forecast found", alert.TextContent);
        Assert.Contains("Zzqxnotacity999", alert.TextContent);
        Assert.Null(document.QuerySelector("article.forecast-result"));
    }

    [Fact]
    public async Task Forecast_BlankCity_ShowsFieldError()
    {
        HttpResponseMessage response = await _client.GetAsync("/weather/forecast?city=");
        AngleSharp.Dom.IDocument document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "Please enter a city name.",
            document.QuerySelector(".field-error")?.TextContent.Trim());
        Assert.Contains(
            "Please enter a city name.",
            document.QuerySelector(".alert.alert-error")?.TextContent);
        Assert.Null(document.QuerySelector("article.forecast-result"));
    }
}
