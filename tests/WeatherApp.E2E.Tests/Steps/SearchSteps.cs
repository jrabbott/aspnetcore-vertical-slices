using Microsoft.Playwright;
using Reqnroll;
using WeatherApp.E2E.Tests.Pages;
using WeatherApp.E2E.Tests.Support;

namespace WeatherApp.E2E.Tests.Steps;

[Binding]
public sealed class SearchSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IPage Page => _scenarioContext.Get<IPage>();

    private WeatherAppEnvironment Environment => _scenarioContext.Get<WeatherAppEnvironment>();

    private SearchPage Search => new(Page, Environment.BaseUrl);

    [Given("I am on the search page")]
    public async Task GivenIAmOnTheSearchPage()
    {
        await Search.OpenAsync().ConfigureAwait(false);
    }

    [When("I search for {string}")]
    public async Task WhenISearchFor(string city)
    {
        await Search.SearchAsync(city).ConfigureAwait(false);
    }

    [Then("I should see current weather for {string}")]
    public async Task ThenIShouldSeeCurrentWeatherFor(string city)
    {
        await Assertions.Expect(Search.ResultHeading).ToContainTextAsync(city).ConfigureAwait(false);
        await Assertions.Expect(Search.Summary).ToHaveTextAsync(E2EConfiguration.StubSummary).ConfigureAwait(false);
        await Assertions.Expect(Page.Locator("article.weather-result")).ToContainTextAsync($"{E2EConfiguration.StubTemperatureC}°C").ConfigureAwait(false);
    }

    [When("I open the forecast from the search result")]
    public async Task WhenIOpenTheForecastFromTheSearchResult()
    {
        await Search.ViewForecastLink.ClickAsync().ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }
}
