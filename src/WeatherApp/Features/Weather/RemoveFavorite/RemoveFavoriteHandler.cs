using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteHandler
{
    private readonly IFavoritesStore _favoritesStore;

    public RemoveFavoriteHandler(IFavoritesStore favoritesStore)
    {
        _favoritesStore = favoritesStore;
    }

    public RemoveFavoriteResult Handle(RemoveFavoriteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.City))
        {
            return RemoveFavoriteResult.Fail("A city is required to remove a favorite.");
        }

        var city = request.City.Trim();
        var removed = _favoritesStore.Remove(city);

        if (!removed)
        {
            return RemoveFavoriteResult.Fail($"{city} was not in your favorites.");
        }

        return RemoveFavoriteResult.Ok($"{city} was removed from your favorites.");
    }
}

public sealed class RemoveFavoriteResult
{
    public required bool Succeeded { get; init; }
    public required string Message { get; init; }

    public static RemoveFavoriteResult Ok(string message) => new() { Succeeded = true, Message = message };
    public static RemoveFavoriteResult Fail(string message) => new() { Succeeded = false, Message = message };
}
