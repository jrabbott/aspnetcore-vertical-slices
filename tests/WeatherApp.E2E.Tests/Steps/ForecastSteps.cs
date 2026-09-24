using Microsoft.Playwright;
using Reqnroll;
using WeatherApp.E2E.Tests.Pages;
using WeatherApp.E2E.Tests.Support;

namespace WeatherApp.E2E.Tests.Steps;

[Binding]
public sealed class ForecastSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IPage Page => _scenarioContext.Get<IPage>();

    private WeatherAppEnvironment Environment => _scenarioContext.Get<WeatherAppEnvironment>();

    private ForecastPage Forecast => new(Page, Environment.BaseUrl);

    [Given("I am on the forecast page")]
    public async Task GivenIAmOnTheForecastPage()
    {
        await Forecast.OpenAsync().ConfigureAwait(false);
    }

    [When("I request a {int}-day forecast for {string}")]
    public async Task WhenIRequestADayForecastFor(int days, string city)
    {
        await Forecast.GetForecastAsync(city, days).ConfigureAwait(false);
    }

    [Then("I should see a forecast for {string}")]
    public async Task ThenIShouldSeeAForecastFor(string city)
    {
        await Assertions.Expect(Forecast.ResultHeading).ToContainTextAsync(city).ConfigureAwait(false);
        await Assertions.Expect(Forecast.ForecastItems).Not.ToHaveCountAsync(0).ConfigureAwait(false);
    }
}
