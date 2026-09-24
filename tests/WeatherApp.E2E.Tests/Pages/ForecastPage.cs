using System.Globalization;
using Microsoft.Playwright;

namespace WeatherApp.E2E.Tests.Pages;

internal sealed class ForecastPage(IPage page, string baseUrl) : BasePage(page, baseUrl)
{
    public ILocator CityInput => Page.GetByLabel("City");

    public ILocator DaysInput => Page.GetByLabel("Days");

    public ILocator GetForecastButton => Page.GetByRole(AriaRole.Button, new()
    {
        Name = "Get forecast"
    });

    public ILocator ResultHeading => Page.Locator("article.forecast-result h2");

    public ILocator ForecastItems => Page.Locator("article.forecast-result ul.forecast-list > li");

    public Task OpenAsync()
    {
        return GotoPathAsync("/weather/forecast");
    }

    public async Task GetForecastAsync(string city, int days = 3)
    {
        await CityInput.FillAsync(city).ConfigureAwait(false);
        await DaysInput.FillAsync(days.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await GetForecastButton.ClickAsync().ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }
}
