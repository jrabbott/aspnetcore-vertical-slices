using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class ForecastEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly HttpClient _client;

    public ForecastEndpointTests(WeatherAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Forecast_WithoutCity_ShowsEmptyForm()
    {
        var response = await _client.GetAsync("/weather/forecast");
        var document = await HtmlDocument.ParseAsync(response);

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
        var response = await _client.GetAsync("/weather/forecast?city=Paris&days=3");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = document.QuerySelector("article.forecast-result");
        Assert.NotNull(result);
        Assert.Equal("Paris, France", result.QuerySelector("h2")?.TextContent.Trim());

        var days = result.QuerySelectorAll("ul.forecast-list > li");
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
        var response = await _client.GetAsync("/weather/forecast?city=Nowhere");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var alert = document.QuerySelector(".alert.alert-error");
        Assert.NotNull(alert);
        Assert.Contains("No forecast found", alert.TextContent);
        Assert.Contains("Nowhere", alert.TextContent);
        Assert.Null(document.QuerySelector("article.forecast-result"));
    }

    [Fact]
    public async Task Forecast_BlankCity_ShowsFieldError()
    {
        var response = await _client.GetAsync("/weather/forecast?city=");
        var document = await HtmlDocument.ParseAsync(response);

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
