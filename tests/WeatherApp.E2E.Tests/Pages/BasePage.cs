using Microsoft.Playwright;

namespace WeatherApp.E2E.Tests.Pages;

internal abstract class BasePage(IPage page, string baseUrl)
{
    protected IPage Page { get; } = page;

    protected string BaseUrl { get; } = baseUrl.TrimEnd('/');

    public ILocator NavSearch => Page.GetByRole(AriaRole.Navigation, new()
    {
        Name = "Main"
    }).GetByRole(AriaRole.Link, new()
    {
        Name = "Search"
    });

    public ILocator NavForecast => Page.GetByRole(AriaRole.Navigation, new()
    {
        Name = "Main"
    }).GetByRole(AriaRole.Link, new()
    {
        Name = "Forecast"
    });

    public ILocator NavFavourites => Page.GetByRole(AriaRole.Navigation, new()
    {
        Name = "Main"
    }).GetByRole(AriaRole.Link, new()
    {
        Name = "Favourites"
    });

    public ILocator Heading => Page.GetByRole(AriaRole.Heading, new()
    {
        Level = 1
    });

    protected async Task GotoPathAsync(string path)
    {
        string relative = path.StartsWith('/') ? path : "/" + path;
        await Page.GotoAsync(BaseUrl + relative, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        }).ConfigureAwait(false);
    }
}
