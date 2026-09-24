using Microsoft.Playwright;

namespace WeatherApp.E2E.Tests.Pages;

internal sealed class FavouritesPage(IPage page, string baseUrl) : BasePage(page, baseUrl)
{
    public ILocator CityInput => Page.Locator("section.add-favourite input#City");

    public ILocator AddFavouriteButton => Page.GetByRole(AriaRole.Button, new()
    {
        Name = "Add favourite",
        Exact = true
    });

    public ILocator FavouriteItems => Page.Locator("ul.favourites-list > li");

    public ILocator EmptyMessage => Page.Locator("p.empty");

    public Task OpenAsync()
    {
        return GotoPathAsync("/weather/favourites");
    }

    public async Task AddFavouriteAsync(string city)
    {
        await CityInput.FillAsync(city).ConfigureAwait(false);
        await AddFavouriteButton.ClickAsync().ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }

    public async Task RemoveFavouriteAsync(string city)
    {
        ILocator row = FavouriteItems.Filter(new()
        {
            HasText = city
        });
        await row.GetByRole(AriaRole.Button, new()
        {
            Name = "Remove"
        }).ClickAsync().ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }

    public ILocator FavouriteNamed(string city)
    {
        return FavouriteItems.Filter(new()
        {
            HasText = city
        });
    }
}
