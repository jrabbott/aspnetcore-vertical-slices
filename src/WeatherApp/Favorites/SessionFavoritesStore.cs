using System.Text.Json;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Favorites;

/// <summary>
/// Per-browser favorites list stored in ASP.NET Core session (in-memory distributed cache by default).
/// </summary>
public sealed class SessionFavoritesStore(IHttpContextAccessor httpContextAccessor) : IFavoritesStore
{
    private const string SessionKey = "WeatherApp.Favorites";

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public IReadOnlyList<string> GetAll()
    {
        List<string> cities = Read();
        return cities
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public bool Add(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return false;
        }

        string normalized = city.Trim();
        List<string> cities = Read();

        if (cities.Exists(c => string.Equals(c, normalized, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        cities.Add(normalized);
        Write(cities);
        return true;
    }

    public bool Remove(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return false;
        }

        List<string> cities = Read();
        int index = cities.FindIndex(c =>
            string.Equals(c, city.Trim(), StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            return false;
        }

        cities.RemoveAt(index);
        Write(cities);
        return true;
    }

    private ISession Session
    {
        get
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HTTP context is not available.");

            return httpContext.Session;
        }
    }

    private List<string> Read()
    {
        string? json = Session.GetString(SessionKey);
        return string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }

    private void Write(List<string> cities)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(cities));
    }
}
