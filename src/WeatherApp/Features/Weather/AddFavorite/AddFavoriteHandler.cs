using FluentValidation;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteHandler
{
    private readonly IFavoritesStore _favoritesStore;
    private readonly IWeatherClient _weatherClient;
    private readonly IValidator<AddFavoriteRequest> _validator;

    public AddFavoriteHandler(
        IFavoritesStore favoritesStore,
        IWeatherClient weatherClient,
        IValidator<AddFavoriteRequest> validator)
    {
        _favoritesStore = favoritesStore;
        _weatherClient = weatherClient;
        _validator = validator;
    }

    public async Task<AddFavoriteResponse> HandleAsync(
        AddFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return AddFavoriteResponse.Fail(validation.Errors[0].ErrorMessage);
        }

        var city = request.City!.Trim();
        var reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);

        if (reading is null)
        {
            return AddFavoriteResponse.Fail($"\"{city}\" is not a supported city.");
        }

        var added = _favoritesStore.Add(reading.Location.City);
        if (!added)
        {
            return AddFavoriteResponse.Fail($"{reading.Location.City} is already in your favorites.");
        }

        return AddFavoriteResponse.Ok($"{reading.Location.City} was added to your favorites.");
    }
}
