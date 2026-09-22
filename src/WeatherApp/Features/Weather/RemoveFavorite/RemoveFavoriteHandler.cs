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

    public async Task<RemoveFavoriteResponse> HandleAsync(
        RemoveFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return RemoveFavoriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        var city = request.City!.Trim();
        var removed = _favoritesStore.Remove(city);

        if (!removed)
        {
            return RemoveFavoriteResponse.Fail($"{city} was not in your favorites.");
        }

        return RemoveFavoriteResponse.Ok($"{city} was removed from your favorites.");
    }
}
