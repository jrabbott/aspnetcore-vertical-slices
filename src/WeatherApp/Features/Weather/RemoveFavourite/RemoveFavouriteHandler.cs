using FluentValidation;
using FluentValidation.Results;
using WeatherApp.Infrastructure.Favourites;

namespace WeatherApp.Features.Weather.RemoveFavourite;

public sealed class RemoveFavouriteHandler(
    IFavouritesStore favouritesStore,
    IValidator<RemoveFavouriteRequest> validator)
{
    private readonly IFavouritesStore _favouritesStore = favouritesStore;
    private readonly IValidator<RemoveFavouriteRequest> _validator = validator;

    public async Task<RemoveFavouriteResponse> HandleAsync(
        RemoveFavouriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidationResult validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return RemoveFavouriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        string city = request.City!.Trim();
        bool removed = _favouritesStore.Remove(city);

        return !removed
            ? RemoveFavouriteResponse.Fail($"{city} was not in your favourites.")
            : RemoveFavouriteResponse.Ok($"{city} was removed from your favourites.");
    }
}
