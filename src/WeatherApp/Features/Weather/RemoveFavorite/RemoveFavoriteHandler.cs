using FluentValidation;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteHandler
{
    private readonly IFavoritesStore _favoritesStore;
    private readonly IValidator<RemoveFavoriteRequest> _validator;

    public RemoveFavoriteHandler(
        IFavoritesStore favoritesStore,
        IValidator<RemoveFavoriteRequest> validator)
    {
        _favoritesStore = favoritesStore;
        _validator = validator;
    }

    public async Task<RemoveFavoriteResult> HandleAsync(
        RemoveFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return RemoveFavoriteResult.Fail(validation.Errors[0].ErrorMessage);
        }

        var city = request.City!.Trim();
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
