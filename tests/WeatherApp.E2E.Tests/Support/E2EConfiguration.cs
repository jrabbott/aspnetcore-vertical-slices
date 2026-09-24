namespace WeatherApp.E2E.Tests.Support;

internal static class E2EConfiguration
{
    public const string BaseUrlEnvironmentVariable = "E2E_BASE_URL";
    public const string ImageEnvironmentVariable = "E2E_IMAGE";
    public const string DefaultImage = "weatherapp:ci";

    public static string? DeployedBaseUrl =>
        NormalizeBaseUrl(Environment.GetEnvironmentVariable(BaseUrlEnvironmentVariable));

    public static bool UseDeployedSite => !string.IsNullOrWhiteSpace(DeployedBaseUrl);

    public static string Image
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable(ImageEnvironmentVariable);
            return string.IsNullOrWhiteSpace(value) ? DefaultImage : value.Trim();
        }
    }

    public static string StubCity => "London";

    public static string StubCountry => "United Kingdom";

    public static string StubSummary => "Clear";

    public static int StubTemperatureC => 21;

    private static string? NormalizeBaseUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().TrimEnd('/');
    }
}
