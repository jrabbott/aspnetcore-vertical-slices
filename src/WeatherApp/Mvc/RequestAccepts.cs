namespace WeatherApp.Mvc;

/// <summary>
/// Content-negotiation helpers for progressive enhancement (HTML first, JSON when requested).
/// </summary>
public static class RequestAccepts
{
    public static bool Json(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string? accept = request.Headers.Accept;
        return !string.IsNullOrWhiteSpace(accept)
            && accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }
}
