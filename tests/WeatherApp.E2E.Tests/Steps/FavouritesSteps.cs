using Microsoft.Playwright;
using Reqnroll;
using WeatherApp.E2E.Tests.Pages;
using WeatherApp.E2E.Tests.Support;

namespace WeatherApp.E2E.Tests.Steps;

[Binding]
public sealed class FavouritesSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IPage Page => _scenarioContext.Get<IPage>();

    private WeatherAppEnvironment Environment => _scenarioContext.Get<WeatherAppEnvironment>();

    private FavouritesPage Favourites => new(Page, Environment.BaseUrl);

    [Given("I am on the favourites page")]
    public async Task GivenIAmOnTheFavouritesPage()
    {
        await Favourites.OpenAsync().ConfigureAwait(false);
    }

    [When("I add {string} as a favourite")]
    public async Task WhenIAddAsAFavourite(string city)
    {
        await Favourites.AddFavouriteAsync(city).ConfigureAwait(false);
    }

    [When("I remove the favourite {string}")]
    public async Task WhenIRemoveTheFavourite(string city)
    {
        await Favourites.RemoveFavouriteAsync(city).ConfigureAwait(false);
    }

    [Then("I should see {string} in my favourites")]
    public async Task ThenIShouldSeeInMyFavourites(string city)
    {
        await Assertions.Expect(Favourites.FavouriteNamed(city)).ToBeVisibleAsync().ConfigureAwait(false);
        await Assertions.Expect(Favourites.FavouriteNamed(city)).ToContainTextAsync(E2EConfiguration.StubSummary).ConfigureAwait(false);
    }

    [Then("I should not see {string} in my favourites")]
    public async Task ThenIShouldNotSeeInMyFavourites(string city)
    {
        await Assertions.Expect(Favourites.FavouriteNamed(city)).ToHaveCountAsync(0).ConfigureAwait(false);
    }
}
