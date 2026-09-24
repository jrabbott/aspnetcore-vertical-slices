using Microsoft.Playwright;

namespace WeatherApp.E2E.Tests.Support;

internal sealed class PlaywrightBrowser : IAsyncDisposable
{
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;

    private PlaywrightBrowser(IPlaywright playwright, IBrowser browser)
    {
        _playwright = playwright;
        _browser = browser;
    }

    public static async Task<PlaywrightBrowser> LaunchAsync()
    {
        string[] installArgs = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase)
            ? ["install", "--with-deps", "chromium"]
            : ["install", "chromium"];
        int installExitCode = Microsoft.Playwright.Program.Main(installArgs);
        if (installExitCode != 0)
        {
            throw new InvalidOperationException($"Playwright browser install failed with exit code {installExitCode}.");
        }

        IPlaywright playwright = await Playwright.CreateAsync().ConfigureAwait(false);
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        }).ConfigureAwait(false);
        return new PlaywrightBrowser(playwright, browser);
    }

    public Task<IBrowserContext> NewContextAsync()
    {
        return _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync().ConfigureAwait(false);
        _playwright.Dispose();
    }
}
