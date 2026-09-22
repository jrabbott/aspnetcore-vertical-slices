using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Features.Weather.RemoveFavorite;

public sealed class RemoveFavoriteHandler(
    IFavoritesStore favoritesStore,
    IValidator<RemoveFavoriteRequest> validator)
{
    private readonly IFavoritesStore _favoritesStore = favoritesStore;
    private readonly IValidator<RemoveFavoriteRequest> _validator = validator;

    public async Task<RemoveFavoriteResponse> HandleAsync(
        RemoveFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return RemoveFavoriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        string city = request.City!.Trim();
        bool removed = _favoritesStore.Remove(city);

        return !removed
            ? RemoveFavoriteResponse.Fail($"{city} was not in your favorites.")
            : RemoveFavoriteResponse.Ok($"{city} was removed from your favorites.");
    }
}
