using Microsoft.Playwright;

namespace WeatherApp.E2E.Tests.Pages;

internal sealed class SearchPage(IPage page, string baseUrl) : BasePage(page, baseUrl)
{
    public ILocator CityInput => Page.GetByLabel("City");

    public ILocator SearchButton => Page.GetByRole(AriaRole.Button, new()
    {
        Name = "Search"
    });

    public ILocator ResultHeading => Page.Locator("article.weather-result h2");

    public ILocator Summary => Page.Locator("article.weather-result .summary");

    public ILocator ViewForecastLink => Page.GetByRole(AriaRole.Link, new()
    {
        Name = "View forecast"
    });

    public ILocator AddToFavouritesButton => Page.GetByRole(AriaRole.Button, new()
    {
        Name = "Add to favourites"
    });

    public Task OpenAsync()
    {
        return GotoPathAsync("/weather/search");
    }

    public async Task SearchAsync(string city)
    {
        await CityInput.FillAsync(city).ConfigureAwait(false);
        await SearchButton.ClickAsync().ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }
}
