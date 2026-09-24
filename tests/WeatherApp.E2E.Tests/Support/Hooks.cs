using Microsoft.Playwright;
using Reqnroll;

namespace WeatherApp.E2E.Tests.Support;

[Binding]
public sealed class Hooks(ScenarioContext scenarioContext)
{
    private static WeatherAppEnvironment? s_environment;
    private static PlaywrightBrowser? s_browser;

    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private IBrowserContext? _context;

    internal static WeatherAppEnvironment Environment =>
        s_environment ?? throw new InvalidOperationException("E2E environment has not started.");

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        s_environment = await WeatherAppEnvironment.StartAsync().ConfigureAwait(false);
        s_browser = await PlaywrightBrowser.LaunchAsync().ConfigureAwait(false);
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (s_browser is not null)
        {
            await s_browser.DisposeAsync().ConfigureAwait(false);
            s_browser = null;
        }

        if (s_environment is not null)
        {
            await s_environment.DisposeAsync().ConfigureAwait(false);
            s_environment = null;
        }
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        PlaywrightBrowser browser = s_browser ?? throw new InvalidOperationException("Playwright browser has not started.");
        _context = await browser.NewContextAsync().ConfigureAwait(false);
        IPage page = await _context.NewPageAsync().ConfigureAwait(false);
        _scenarioContext.Set(page);
        _scenarioContext.Set(Environment);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_context is not null)
        {
            await _context.CloseAsync().ConfigureAwait(false);
            _context = null;
        }
    }
}
