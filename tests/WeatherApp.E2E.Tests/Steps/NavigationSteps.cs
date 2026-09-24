using Microsoft.Playwright;
using Reqnroll;
using WeatherApp.E2E.Tests.Pages;
using WeatherApp.E2E.Tests.Support;

namespace WeatherApp.E2E.Tests.Steps;

[Binding]
public sealed class NavigationSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IPage Page => _scenarioContext.Get<IPage>();

    private WeatherAppEnvironment Environment => _scenarioContext.Get<WeatherAppEnvironment>();

    [Given("I open the weather app")]
    public async Task GivenIOpenTheWeatherApp()
    {
        await Page.GotoAsync(Environment.BaseUrl + "/", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        }).ConfigureAwait(false);
    }

    [Then("I should be on the search page")]
    public async Task ThenIShouldBeOnTheSearchPage()
    {
        string path = new Uri(Page.Url).AbsolutePath;
        Assert.Equal("/weather/search", path);
        SearchPage search = new(Page, Environment.BaseUrl);
        await Assertions.Expect(search.Heading).ToHaveTextAsync("Search weather").ConfigureAwait(false);
    }
}
