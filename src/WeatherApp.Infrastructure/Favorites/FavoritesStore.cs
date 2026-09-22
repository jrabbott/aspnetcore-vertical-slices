namespace WeatherApp.Infrastructure.Favorites;

/// <summary>
/// Process-wide in-memory favorites store for the sample application.
/// </summary>
public sealed class FavoritesStore(IEnumerable<string> initialCities) : IFavoritesStore
{
    private readonly object _gate = new();
    private readonly List<string> _cities = [.. initialCities
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)];

    public FavoritesStore()
        : this(["London", "Tokyo"])
    {
    }

    public IReadOnlyList<string> GetAll()
    {
        lock (_gate)
        {
            return _cities
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }

    public bool Add(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return false;
        }

        string normalized = city.Trim();

        lock (_gate)
        {
            if (_cities.Exists(c => string.Equals(c, normalized, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            _cities.Add(normalized);
            return true;
        }
    }

    public bool Remove(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return false;
        }

        lock (_gate)
        {
            int index = _cities.FindIndex(c =>
                string.Equals(c, city.Trim(), StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                return false;
            }

            _cities.RemoveAt(index);
            return true;
        }
    }
}
